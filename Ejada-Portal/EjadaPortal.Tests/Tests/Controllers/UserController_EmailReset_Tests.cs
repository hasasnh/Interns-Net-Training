using Application.Services.IServices;
using Ejada_Portal.Controllers;
using Ejada_Portal.Models;
using EjadaPortal.Tests.Helpers;
using FluentAssertions;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Moq;
using Xunit;

namespace EjadaPortal.Tests.Tests.Controllers
{
    public class UserController_EmailReset_Tests
    {
        //  POSITIVE (5) 

        [Fact]
        public void ForgotPassword_GET_ReturnsView_WithDefaultGmail()
        {
            // Arrange
            var userServiceMock = new Mock<IUserService>();
            var controller = ControllerTestHelper.CreateControllerWithContext(userServiceMock, isAuthenticated: false);

            // Act
            var result = controller.ForgotPassword((string?)null) as ViewResult;

            // Assert
            result.Should().NotBeNull();
            var model = result!.Model as ForgotPasswordViewModel;
            model.Should().NotBeNull();
            model!.SelectedProvider.Should().Be("Gmail");
        }

        [Fact]
        public async Task ForgotPassword_POST_Gmail_Success_RedirectsAndSetsTempData()
        {
            // Arrange
            var userServiceMock = new Mock<IUserService>();
            userServiceMock
                .Setup(s => s.SendPasswordResetLinkAsync("u@x.com", It.IsAny<string>(), "Gmail"))
                .ReturnsAsync(true);

            var controller = ControllerTestHelper.CreateControllerWithContext(userServiceMock, isAuthenticated: false);
            var model = new ForgotPasswordViewModel { Email = "u@x.com", SelectedProvider = "Gmail" };

            // Act
            var result = await controller.ForgotPassword(model) as RedirectToActionResult;

            // Assert
            result.Should().NotBeNull();
            result!.ActionName.Should().Be(nameof(UserController.ForgotPassword));
            controller.TempData["MailSent"].Should().NotBeNull();
        }

        [Fact]
        public async Task ForgotPassword_POST_Rnwood_Success_CallsServiceWithProvider()
        {
            // Arrange
            var userServiceMock = new Mock<IUserService>(MockBehavior.Strict);
            userServiceMock
                .Setup(s => s.SendPasswordResetLinkAsync("u@x.com", It.IsAny<string>(), "Rnwood"))
                .ReturnsAsync(true)
                .Verifiable();

            var controller = ControllerTestHelper.CreateControllerWithContext(userServiceMock, isAuthenticated: true);
            var model = new ForgotPasswordViewModel { Email = "u@x.com", SelectedProvider = "Rnwood" };

            // Act
            var result = await controller.ForgotPassword(model) as RedirectToActionResult;

            // Assert
            result.Should().NotBeNull();
            result!.ActionName.Should().Be(nameof(UserController.ForgotPassword));
            userServiceMock.Verify();
        }

        [Fact]
        public void SelectProvider_GET_ReturnsView_WithAvailableProviders()
        {
            // Arrange
            var userServiceMock = new Mock<IUserService>();
            var controller = ControllerTestHelper.CreateControllerWithContext(userServiceMock, isAuthenticated: true);

            // Act
            var result = controller.SelectProvider() as ViewResult;

            // Assert
            result.Should().NotBeNull();
            var model = result!.Model as SelectProviderViewModel;
            model.Should().NotBeNull();
            model!.AvailableProviders.Should().Contain(new[] { "Gmail", "Rnwood" });
        }

        [Fact]
        public void SelectProvider_POST_Valid_RedirectsToForgotPasswordWithProvider()
        {
            // Arrange
            var userServiceMock = new Mock<IUserService>();
            var controller = ControllerTestHelper.CreateControllerWithContext(userServiceMock, isAuthenticated: true);

            var model = new SelectProviderViewModel { SelectedProvider = "Gmail" };

            // Act
            var result = controller.SelectProvider(model) as RedirectToActionResult;

            // Assert
            result.Should().NotBeNull();
            result!.ActionName.Should().Be(nameof(UserController.ForgotPassword));
            result.RouteValues.Should().ContainKey("provider").WhoseValue.Should().Be("Gmail");
        }
        [Fact]
        public async Task ResetPassword_POST_Success_RedirectsAndSetsTempData()
        {
            // Arrange
            var userServiceMock = new Mock<IUserService>();
            userServiceMock
                .Setup(s => s.ResetPasswordAsync("u@x.com", "TK", "12345678", "Gmail"))
                .ReturnsAsync(IdentityResult.Success);

            var controller = ControllerTestHelper.CreateControllerWithContext(userServiceMock, isAuthenticated: false);
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
            result!.ActionName.Should().Be(nameof(UserController.ForgotPassword));
            controller.TempData["PasswordChanged"].Should().NotBeNull();
        }
        // ========== NEGATIVE (5) ==========

        [Fact]
        public async Task ForgotPassword_POST_InvalidModel_ReturnsView()
        {
            // Arrange
            var userServiceMock = new Mock<IUserService>();
            var controller = ControllerTestHelper.CreateControllerWithContext(userServiceMock, isAuthenticated: false);
            controller.ModelState.AddModelError(nameof(ForgotPasswordViewModel.Email), "Required");
            var model = new ForgotPasswordViewModel { Email = "", SelectedProvider = "Gmail" };

            // Act
            var result = await controller.ForgotPassword(model) as ViewResult;

            // Assert
            result.Should().NotBeNull();
            result!.ViewData.ModelState.IsValid.Should().BeFalse();
        }

        [Fact]
        public async Task ForgotPassword_POST_UserNotFound_ReturnsViewWithEmailError()
        {
            // Arrange
            var userServiceMock = new Mock<IUserService>();
            userServiceMock
                .Setup(s => s.SendPasswordResetLinkAsync("no@x.com", It.IsAny<string>(), "Gmail"))
                .ReturnsAsync(false);

            var controller = ControllerTestHelper.CreateControllerWithContext(userServiceMock, isAuthenticated: false);
            var model = new ForgotPasswordViewModel { Email = "no@x.com", SelectedProvider = "Gmail" };

            // Act
            var result = await controller.ForgotPassword(model) as ViewResult;

            // Assert
            result.Should().NotBeNull();
            result!.ViewData.ModelState[nameof(model.Email)]!.Errors.Should().NotBeEmpty();
        }

        [Fact]
        public async Task ForgotPassword_POST_AuthenticatedUser_UsesSelectedProvider()
        {
            // Arrange
            var userServiceMock = new Mock<IUserService>(MockBehavior.Strict);
            userServiceMock
                .Setup(s => s.SendPasswordResetLinkAsync("auth@x.com", It.IsAny<string>(), "Rnwood"))
                .ReturnsAsync(true)
                .Verifiable();

            var controller = ControllerTestHelper.CreateControllerWithContext(userServiceMock, isAuthenticated: true);
            var model = new ForgotPasswordViewModel
            {
                Email = "auth@x.com",
                SelectedProvider = "Rnwood"
            };

            // Act
            var result = await controller.ForgotPassword(model) as RedirectToActionResult;

            // Assert
            result.Should().NotBeNull();
            result!.ActionName.Should().Be(nameof(UserController.ForgotPassword));
            userServiceMock.Verify(); // Make sure it was sent via Rnwood, not Gmail
        }


        [Fact]
        public async Task ResetPassword_POST_PasswordMismatch_ReturnsViewWithConfirmError()
        {
            // Arrange
            var userServiceMock = new Mock<IUserService>();
            var controller = ControllerTestHelper.CreateControllerWithContext(userServiceMock, isAuthenticated: false);

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
        public async Task ResetPassword_POST_ServiceFailed_ReturnsViewWithErrors()
        {
            // Arrange
            var userServiceMock = new Mock<IUserService>();
            var failure = IdentityResult.Failed(new IdentityError { Description = "Bad token" });

            userServiceMock
                .Setup(s => s.ResetPasswordAsync(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>()))
                .ReturnsAsync(failure);

            var controller = ControllerTestHelper.CreateControllerWithContext(userServiceMock, isAuthenticated: false);
            var model = new ResetPasswordViewModel
            {
                Email = "u@x.com",
                Token = "ENC_T",
                Password = "12345678",
                ConfirmPassword = "12345678",
                SelectedProvider = "Gmail"
            };

            // Act
            var result = await controller.ResetPassword(model) as ViewResult;

            // Assert
            result.Should().NotBeNull();
            result!.ViewData.ModelState[string.Empty].Errors.Should().ContainSingle(e => e.ErrorMessage.Contains("Bad token"));
        }

        
    }
}
