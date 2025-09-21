using System.Security.Claims;
using Application.ServiceManager;
using Application.Services.IServices;
using Ejada_Portal.Controllers;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Routing;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Moq;

namespace EjadaPortal.Tests.Helpers
{
    public static class ControllerTestHelper
    {
        public static UserController CreateControllerWithContext(Mock<IUserService> userServiceMock,bool isAuthenticated = false)
        {
            var serviceManagerMock = new Mock<IServiceManager>();
            serviceManagerMock.Setup(s => s.UserService).Returns(userServiceMock.Object);

            var controller = new UserController(serviceManagerMock.Object);

            //fake HttpContext
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

            controller.ControllerContext = new ControllerContext { HttpContext = httpContext };

            // TempData
            controller.TempData = new TempDataDictionary(httpContext, Mock.Of<ITempDataProvider>());

            // Fake UrlHelper
            var urlHelperMock = new Mock<IUrlHelper>(MockBehavior.Strict);
            urlHelperMock
                .Setup(u => u.Action(It.IsAny<UrlActionContext>()))
                .Returns((UrlActionContext ctx) =>
                {
                    var protocol = ctx.Protocol ?? "http";
                    var action = ctx.Action ?? "Index";
                    var controllerName = ctx.Controller ?? "User";
                    return $"{protocol}://app/{controllerName}/{action}";
                });
            controller.Url = urlHelperMock.Object;

            return controller;
        }
    }
}
