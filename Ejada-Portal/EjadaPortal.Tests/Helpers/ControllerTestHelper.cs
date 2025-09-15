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
        public static UserController CreateControllerWithContext(Mock<IUserService> userServiceMock)
        {
            var serviceManagerMock = new Mock<IServiceManager>();
            serviceManagerMock.Setup(sm => sm.UserService).Returns(userServiceMock.Object);

            var controller = new UserController(serviceManagerMock.Object);

            var httpContext = new DefaultHttpContext();

            controller.TempData = new TempDataDictionary(httpContext, Mock.Of<ITempDataProvider>());

            controller.ControllerContext = new ControllerContext
            {
                HttpContext = httpContext
            };

            var urlHelperMock = new Mock<IUrlHelper>();
            urlHelperMock.Setup(x => x.Action(It.IsAny<UrlActionContext>())).Returns("http://localhost/fake");
            controller.Url = urlHelperMock.Object;

            return controller;
        }
    }
}
