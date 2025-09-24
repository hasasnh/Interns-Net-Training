using Application.ServiceManager;
using Application.Services.IServices;
using Ejada_Portal.Controllers;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Routing;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Microsoft.Extensions.DependencyInjection;
using Moq;
using System.Security.Claims;

namespace EjadaPortal.Tests.Helpers
{
    public static class AuthControllerTestHelper
    {
        public static UserController CreateUserControllerWithContext(
            Mock<IUserService>? userServiceMock,
            out Mock<IAuthenticationService> authServiceMock,
            bool isAuthenticated = false)
        {
            var serviceManagerMock = new Mock<IServiceManager>();
            if (userServiceMock != null)
                serviceManagerMock.Setup(s => s.UserService).Returns(userServiceMock.Object);

            var controller = new UserController(serviceManagerMock.Object);

            // configer HttpContext
            var httpContext = new DefaultHttpContext();
            httpContext.Request.Scheme = "http";

            if (isAuthenticated)
            {
                httpContext.User = new ClaimsPrincipal(
                    new ClaimsIdentity(new[] { new Claim(ClaimTypes.Name, "tester") }, "TestAuth"));
            }
            else
            {
                httpContext.User = new ClaimsPrincipal(new ClaimsIdentity());
            }

            // Mock AuthenticationService
            authServiceMock = new Mock<IAuthenticationService>();

            //  Login → AuthenticateAsync
            authServiceMock
                .Setup(a => a.AuthenticateAsync(It.IsAny<HttpContext>(), null))
                .ReturnsAsync(AuthenticateResult.Success(
                    new AuthenticationTicket(
                        new ClaimsPrincipal(new ClaimsIdentity(
                            new[] { new Claim("access_token", "fake-token") }, "mockAuth")),
                        "mockScheme")));

            //  Logout → SignOutAsync
            authServiceMock
                .Setup(a => a.SignOutAsync(It.IsAny<HttpContext>(), null, null))
                .Returns(Task.CompletedTask);

            httpContext.RequestServices = new ServiceCollection()
                .AddSingleton(authServiceMock.Object)
                .BuildServiceProvider();

            controller.ControllerContext = new ControllerContext { HttpContext = httpContext };

            // TempData
            controller.TempData = new TempDataDictionary(httpContext, Mock.Of<ITempDataProvider>());

            // Mock UrlHelper
            var urlHelperMock = new Mock<IUrlHelper>();
            urlHelperMock
                .Setup(u => u.Action(It.IsAny<UrlActionContext>()))
                .Returns("http://localhost/Home/Index");
            controller.Url = urlHelperMock.Object;

            return controller;
        }
    }
}
