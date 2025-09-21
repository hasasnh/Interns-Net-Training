using Application.Services.IServices;
using Ejada_Portal.Controllers;
using Ejada_Portal.Models;
using EjadaPortal.Tests.Helpers;
using FluentAssertions;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Moq;

namespace EjadaPortal.Tests.Tests.Controllers
{
    public class UserController_EmailReset_Tests
    {
        // POSITIVE TESTS (5)

        [Fact]
        public async Task ForgotPassword_GET_DefaultsToGmail()
        {
            // Arrange
            var userServiceMock = new Mock<IUserService>();
            var controller = ControllerTestHelper.CreateControllerWithContext(userServiceMock, false);

            // Act
            var result = await controller.ForgotPassword((string?)null) as ViewResult;

            // Assert
            result.Should().NotBeNull();
            var model = result!.Model as ForgotPasswordViewModel;
            model!.SelectedProvider.Should().Be("Gmail");
        }

        [Fact]
        public async Task ForgotPassword_POST_Gmail_Success()
        {
            // Arrange
            var userServiceMock = new Mock<IUserService>();
            userServiceMock.Setup(s => s.SendPasswordResetLinkAsync("u@x.com", It.IsAny<string>(), "Gmail"))
                .ReturnsAsync(true);

            var controller = ControllerTestHelper.CreateControllerWithContext(userServiceMock, false);
            var model = new ForgotPasswordViewModel { Email = "u@x.com", SelectedProvider = "Gmail" };

            // Act
            var result = await controller.ForgotPassword(model) as RedirectToActionResult;

            // Assert
            result.Should().NotBeNull();
            result!.ActionName.Should().Be(nameof(UserController.ForgotPassword));
            controller.TempData["MailSent"].Should().NotBeNull();
        }

        [Fact]
        public async Task ForgotPassword_POST_Rnwood_Success()
        {
            // Arrange
            var userServiceMock = new Mock<IUserService>(MockBehavior.Strict);
            userServiceMock.Setup(s => s.SendPasswordResetLinkAsync("auth@x.com", It.IsAny<string>(), "Rnwood"))
                .ReturnsAsync(true)
                .Verifiable();

            var controller = ControllerTestHelper.CreateControllerWithContext(userServiceMock, true);
            var model = new ForgotPasswordViewModel { Email = "auth@x.com", SelectedProvider = "Rnwood" };

            // Act
            var result = await controller.ForgotPassword(model) as RedirectToActionResult;

            // Assert
            result.Should().NotBeNull();
            userServiceMock.Verify();
        }

        [Fact]
        public async Task SelectProvider_GET_ReturnsProviders()
        {
            // Arrange
            var userServiceMock = new Mock<IUserService>();
            var controller = ControllerTestHelper.CreateControllerWithContext(userServiceMock, true);

            // Act
            var result = await controller.SelectProvider() as ViewResult;

            // Assert
            result.Should().NotBeNull();
            var model = result!.Model as SelectProviderViewModel;
            model!.AvailableProviders.Should().Contain(new[] { "Gmail", "Rnwood", "Hotmail" });
        }

        [Fact]
        public async Task ResetPassword_POST_Success()
        {
            // Arrange
            var userServiceMock = new Mock<IUserService>();
            userServiceMock
                .Setup(s => s.ResetPasswordAsync("u@x.com", "TK", "12345678", "Gmail"))
                .ReturnsAsync(IdentityResult.Success);

            var controller = ControllerTestHelper.CreateControllerWithContext(userServiceMock, false);
            var model = new ResetPasswordViewModel
            {
                Email = "u@x.com",
                Token = "TK",
                Password = "12345678",
                ConfirmPassword = "12345678",
                SelectedProvider = "Gmail"
            };

            // Act
            var result = await controller.ResetPassword(model) as RedirectToActionResult;

            // Assert
            result.Should().NotBeNull();
            controller.TempData["PasswordChanged"].Should().NotBeNull();
        }

        // NEGATIVE TESTS (5)

        [Fact]
        public async Task ForgotPassword_POST_InvalidModel_ReturnsView()
        {
            // Arrange
            var userServiceMock = new Mock<IUserService>();
            var controller = ControllerTestHelper.CreateControllerWithContext(userServiceMock, false);
            controller.ModelState.AddModelError(nameof(ForgotPasswordViewModel.Email), "Required");
            var model = new ForgotPasswordViewModel { Email = "", SelectedProvider = "Gmail" };

            // Act
            var result = await controller.ForgotPassword(model) as ViewResult;

            // Assert
            result.Should().NotBeNull();
            result!.ViewData.ModelState.IsValid.Should().BeFalse();
        }

        [Fact]
        public async Task ForgotPassword_POST_UserNotFound()
        {
            // Arrange
            var userServiceMock = new Mock<IUserService>();
            userServiceMock
                .Setup(s => s.SendPasswordResetLinkAsync("no@x.com", It.IsAny<string>(), "Gmail"))
                .ReturnsAsync(false);

            var controller = ControllerTestHelper.CreateControllerWithContext(userServiceMock, false);
            var model = new ForgotPasswordViewModel { Email = "no@x.com", SelectedProvider = "Gmail" };

            // Act
            var result = await controller.ForgotPassword(model) as ViewResult;

            // Assert
            result.Should().NotBeNull();
            result!.ViewData.ModelState[nameof(model.Email)]!.Errors.Should().NotBeEmpty();
        }

        [Fact]
        public async Task ForgotPassword_POST_ServiceThrows_ExceptionHandled()
        {
            // Arrange
            var userServiceMock = new Mock<IUserService>();
            userServiceMock
                .Setup(s => s.SendPasswordResetLinkAsync(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>()))
                .ThrowsAsync(new Exception("SMTP failed"));

            var controller = ControllerTestHelper.CreateControllerWithContext(userServiceMock, false);
            var model = new ForgotPasswordViewModel { Email = "u@x.com", SelectedProvider = "Gmail" };

            // Act
            var result = await controller.ForgotPassword(model) as ViewResult;

            // Assert
            result.Should().NotBeNull();
            controller.TempData["error"].Should().NotBeNull();
        }

        [Fact]
        public async Task ResetPassword_POST_PasswordMismatch()
        {
            // Arrange
            var userServiceMock = new Mock<IUserService>();
            var controller = ControllerTestHelper.CreateControllerWithContext(userServiceMock, false);
            var model = new ResetPasswordViewModel
            {
                Email = "u@x.com",
                Token = "T",
                Password = "12345678",
                ConfirmPassword = "87654321",
                SelectedProvider = "Gmail"
            };

            // Act
            var result = await controller.ResetPassword(model) as ViewResult;

            // Assert
            result.Should().NotBeNull();
            result!.ViewData.ModelState[nameof(model.ConfirmPassword)]!.Errors.Should().NotBeEmpty();
        }

        [Fact]

        public async Task ResetPassword_POST_MissingToken_ReturnsViewWithModelError()
        {
            // Arrange
            var userServiceMock = new Mock<IUserService>();
            var controller = ControllerTestHelper.CreateControllerWithContext(userServiceMock, false);

            var model = new ResetPasswordViewModel
            {
                Email = "u@x.com",
                Token = "", // Token missing
                Password = "12345678",
                ConfirmPassword = "12345678",
                SelectedProvider = "Gmail"
            };

            controller.ModelState.AddModelError(nameof(ResetPasswordViewModel.Token), "Token is required");

            // Act
            var result = await controller.ResetPassword(model) as ViewResult;

            // Assert
            result.Should().NotBeNull();
            result!.ViewData.ModelState[nameof(ResetPasswordViewModel.Token)]!
                .Errors.Should().NotBeEmpty();
        }

    }
}
