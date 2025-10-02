using Application.ServiceManager;
using Application.Services.IServices;
using Ejada_Portal.Controllers;
using EjadaPortal.Tests.Helpers;
using FluentAssertions;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Moq;

namespace EjadaPortal.Tests.Tests.Controllers
{
    public class UserController_Auth_Tests
    {
        [Fact]
        public async Task Login_WhenAuthenticated_ShouldRedirectToHomeIndex()
        {
            // Arrange
            var userServiceMock = new Mock<IUserService>();
            var controller = AuthControllerTestHelper.CreateUserControllerWithContext(
                userServiceMock, out var authServiceMock, isAuthenticated: true);

            // Act
            var result = await controller.Login();

            // Assert
            result.Should().BeOfType<RedirectToActionResult>();
            var redirect = result as RedirectToActionResult;
            redirect!.ActionName.Should().Be("Index");
            redirect.ControllerName.Should().Be("Home");

            // Verify call - Verifies that AuthenticateAsync was called once.
            authServiceMock.Verify(a =>
                a.AuthenticateAsync(controller.HttpContext, null), Times.Once);
        }

        [Fact]
        public async Task Login_WhenNoToken_ShouldStillRedirectToHomeIndex()
        {
            //Arrange
            var userServiceMock = new Mock<IUserService>();
            var controller = AuthControllerTestHelper.CreateUserControllerWithContext(
                userServiceMock, out var authServiceMock, true);

            // Fakes AuthenticateAsync returning null (no token)
            authServiceMock
                .Setup(a => a.AuthenticateAsync(It.IsAny<HttpContext>(), null))
                .ReturnsAsync((AuthenticateResult)null);

            //Act
            var result = await controller.Login();

            //Assert
            result.Should().BeOfType<RedirectToActionResult>();
            var redirect = result as RedirectToActionResult;
            redirect!.ActionName.Should().Be("Index");
            redirect.ControllerName.Should().Be("Home");
        }

        [Fact]
        public async Task Login_WhenHttpContextIsNull_ShouldThrow()
        {
            //controller without HttpContext.
            var controller = new UserController(new Mock<IServiceManager>().Object);

            await Assert.ThrowsAsync<NullReferenceException>(() => controller.Login());
        }


        [Fact]
        public async Task Logout_WhenCalled_ShouldSignOutAndRedirectToHomeIndex()
        {
            //Arrange
            var userServiceMock = new Mock<IUserService>();
            var controller = AuthControllerTestHelper.CreateUserControllerWithContext(
                userServiceMock, out var authServiceMock, true);

            //Act
            var result = await controller.Logout();

            //Assert
            result.Should().BeOfType<RedirectToActionResult>();
            var redirect = result as RedirectToActionResult;
            redirect!.ActionName.Should().Be("Index");
            redirect.ControllerName.Should().Be("Home");

            // Verify call
            authServiceMock.Verify(a =>
                a.SignOutAsync(controller.HttpContext, null, null), Times.Once);
        }

        [Fact]
        public async Task Logout_CalledTwice_ShouldStillRedirect()
        {
            var controller = AuthControllerTestHelper.CreateUserControllerWithContext(
                new Mock<IUserService>(), out _, true);

            var result1 = await controller.Logout();
            var result2 = await controller.Logout();

            result1.Should().BeOfType<RedirectToActionResult>();
            result2.Should().BeOfType<RedirectToActionResult>();
        }

        [Fact]
        public async Task Logout_WhenHttpContextIsNull_ShouldThrow()
        {
            var controller = new UserController(new Mock<IServiceManager>().Object);

            await Assert.ThrowsAsync<NullReferenceException>(() => controller.Logout());
        }

        [Fact]
        public async Task Logout_WhenSignOutThrows_ShouldThrow()
        {
            var controller = AuthControllerTestHelper.CreateUserControllerWithContext(
                new Mock<IUserService>(), out var authServiceMock, true);

            authServiceMock
                .Setup(a => a.SignOutAsync(It.IsAny<HttpContext>(), null, null))
                .ThrowsAsync(new InvalidOperationException("SignOut failed"));

            await Assert.ThrowsAsync<InvalidOperationException>(() => controller.Logout());
        }


        [Fact]
        public void Register_WhenCalled_ShouldReturnView()
        {
            // Arrange
            var controller = new UserController(null);

            // Act
            var result = controller.Register();

            // Assert
            result.Should().BeOfType<ViewResult>();
            var viewResult = result as ViewResult;
            viewResult!.ViewName.Should().BeNull();
        }

    }
}
