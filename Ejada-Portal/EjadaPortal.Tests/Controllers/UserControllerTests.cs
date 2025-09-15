using Application.Services.IServices;
using Ejada_Portal.Models;
using EjadaPortal.Tests.Helpers;
using FluentAssertions;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Moq;

public class UserControllerTests
{
    // Positive Scenarios

    [Fact]
    // GET ForgotPassword should return the view
    public void GET_ForgotPassword_ShouldReturnView()
    {
        // Arrange
        var mock = new Mock<IUserService>();
        var controller = ControllerTestHelper.CreateControllerWithContext(mock);

        // Act
        var result = controller.ForgotPassword();

        // Assert
        result.Should().BeOfType<ViewResult>();
    }

    [Fact]
    // POST ForgotPassword with valid email should redirect
    public async Task POST_ForgotPassword_EmailSent_ShouldRedirect()
    {
        // Arrange
        var mock = new Mock<IUserService>();
        mock.Setup(s => s.SendPasswordResetLinkAsync("u@x.com", It.IsAny<string>()))
            .ReturnsAsync(true);

        var controller = ControllerTestHelper.CreateControllerWithContext(mock);
        var model = new ForgotPasswordViewModel { Email = "u@x.com" };

        // Act
        var result = await controller.ForgotPassword(model) as RedirectToActionResult;

        // Assert
        result.Should().NotBeNull();
        result.ActionName.Should().Be("ForgotPassword");
    }

    [Fact]
    // GET ResetPassword with valid parameters should return view with model
    public void GET_ResetPassword_ValidParameters_ShouldReturnViewWithModel()
    {
        // Arrange
        var mock = new Mock<IUserService>();
        var controller = ControllerTestHelper.CreateControllerWithContext(mock);

        // Act
        var result = controller.ResetPassword("u@x.com", "token123") as ViewResult;

        // Assert
        result.Should().NotBeNull();
        result.Model.Should().BeOfType<ResetPasswordViewModel>();
    }

    [Fact]
    // POST ResetPassword with valid passwords should redirect
    public async Task POST_ResetPassword_ValidPasswords_ShouldRedirect()
    {
        // Arrange
        var mock = new Mock<IUserService>();
        mock.Setup(s => s.ResetPasswordAsync(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>()))
            .ReturnsAsync(IdentityResult.Success);

        var controller = ControllerTestHelper.CreateControllerWithContext(mock);
        var model = new ResetPasswordViewModel
        {
            Email = "u@x.com",
            Token = "token123",
            Password = "12345678",
            ConfirmPassword = "12345678"
        };

        // Act
        var result = await controller.ResetPassword(model) as RedirectToActionResult;

        // Assert
        result.Should().NotBeNull();
        result.ActionName.Should().Be("ForgotPassword");
    }

    [Fact]
    // POST ResetPassword when user exists should return success redirect
    public async Task POST_ResetPassword_UserExists_ShouldReturnSuccess()
    {
        // Arrange
        var mock = new Mock<IUserService>();
        mock.Setup(s => s.ResetPasswordAsync("u@x.com", "token123", "12345678"))
            .ReturnsAsync(IdentityResult.Success);

        var controller = ControllerTestHelper.CreateControllerWithContext(mock);
        var model = new ResetPasswordViewModel
        {
            Email = "u@x.com",
            Token = "token123",
            Password = "12345678",
            ConfirmPassword = "12345678"
        };

        // Act
        var result = await controller.ResetPassword(model) as RedirectToActionResult;

        // Assert
        result.Should().NotBeNull();
        result.ActionName.Should().Be("ForgotPassword");
    }

    // Negative Scenarios

    [Fact]
    // POST ForgotPassword with unknown email should return view with model error
    public async Task POST_ForgotPassword_UserNotFound_ShouldReturnViewWithError()
    {
        // Arrange
        var mock = new Mock<IUserService>();
        mock.Setup(s => s.SendPasswordResetLinkAsync(It.IsAny<string>(), It.IsAny<string>()))
            .ReturnsAsync(false);

        var controller = ControllerTestHelper.CreateControllerWithContext(mock);
        var model = new ForgotPasswordViewModel { Email = "unknown@x.com" };

        // Act
        var result = await controller.ForgotPassword(model) as ViewResult;

        // Assert
        result.Should().NotBeNull();
        result.ViewData.ModelState[nameof(model.Email)].Errors.Should().NotBeEmpty();
    }

    [Fact]
    // POST ForgotPassword throws exception should redirect with error message
    public async Task POST_ForgotPassword_ThrowsException_ShouldRedirectWithError()
    {
        // Arrange
        var mock = new Mock<IUserService>();
        mock.Setup(s => s.SendPasswordResetLinkAsync(It.IsAny<string>(), It.IsAny<string>()))
            .ThrowsAsync(new Exception("SMTP error"));

        var controller = ControllerTestHelper.CreateControllerWithContext(mock);
        var model = new ForgotPasswordViewModel { Email = "u@x.com" };

        // Act
        var result = await controller.ForgotPassword(model) as RedirectToActionResult;

        // Assert
        result.Should().NotBeNull();
        controller.TempData["error"].Should().Be("تعذَّر إرسال البريد. تأكّد من إعدادات Gmail (App Password/Host/Port/StartTLS).");
    }

    [Fact]
    // GET ResetPassword with missing parameters should redirect
    public void GET_ResetPassword_MissingParameters_ShouldRedirect()
    {
        // Arrange
        var mock = new Mock<IUserService>();
        var controller = ControllerTestHelper.CreateControllerWithContext(mock);

        // Act
        var result = controller.ResetPassword(null, null) as RedirectToActionResult;

        // Assert
        result.Should().NotBeNull();
        result.ActionName.Should().Be("ForgotPassword");
    }

    [Fact]
    // POST ResetPassword with mismatched passwords should return view with model error
    public async Task POST_ResetPassword_PasswordsMismatch_ShouldReturnViewWithError()
    {
        // Arrange
        var mock = new Mock<IUserService>();
        var controller = ControllerTestHelper.CreateControllerWithContext(mock);

        var model = new ResetPasswordViewModel
        {
            Email = "u@x.com",
            Token = "token123",
            Password = "12345678",
            ConfirmPassword = "87654321"
        };

        // Act
        var result = await controller.ResetPassword(model) as ViewResult;

        // Assert
        result.Should().NotBeNull();
        result.ViewData.ModelState[nameof(model.ConfirmPassword)].Errors.Should().NotBeEmpty();
    }

    [Fact]
    // POST ResetPassword fails should return view with model errors
    public async Task POST_ResetPassword_Failure_ShouldReturnViewWithModelErrors()
    {
        // Arrange
        var mock = new Mock<IUserService>();
        var failedResult = IdentityResult.Failed(new IdentityError { Description = "Password reset failed" });
        mock.Setup(s => s.ResetPasswordAsync(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>()))
            .ReturnsAsync(failedResult);

        var controller = ControllerTestHelper.CreateControllerWithContext(mock);

        var model = new ResetPasswordViewModel
        {
            Email = "u@x.com",
            Token = "token123",
            Password = "12345678",
            ConfirmPassword = "12345678"
        };

        // Act
        var result = await controller.ResetPassword(model) as ViewResult;

        // Assert
        result.Should().NotBeNull();
        result.ViewData.ModelState[string.Empty].Errors.Should().ContainSingle(e => e.ErrorMessage.Contains("Password reset failed"));
    }
}
