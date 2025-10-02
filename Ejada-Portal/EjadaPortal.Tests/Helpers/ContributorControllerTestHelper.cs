using System.Security.Claims;
using Application.ServiceManager;
using Application.Services.IServices;
using Ejada_Portal.Controllers;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Moq;

namespace EjadaPortal.Tests.Helpers
{
    public static class ContributorControllerTestHelper
    {
        public static ContributorController CreateControllerWithContext(Mock<IContributorService> contributorServiceMock, bool isAuthenticated = true)
        {
            var serviceManagerMock = new Mock<IServiceManager>();
            serviceManagerMock.Setup(s => s.ContributorService).Returns(contributorServiceMock.Object);

            var controller = new ContributorController(serviceManagerMock.Object);

            var httpContext = new DefaultHttpContext();
            if (isAuthenticated)
            {
                httpContext.User = new ClaimsPrincipal(
                    new ClaimsIdentity(new[] { new Claim(ClaimTypes.Name, "testuser") }, "TestAuth"));
            }
            else
            {
                httpContext.User = new ClaimsPrincipal(new ClaimsIdentity());
            }

            controller.ControllerContext = new ControllerContext { HttpContext = httpContext };

            // TempData
            controller.TempData = new TempDataDictionary(httpContext, Mock.Of<ITempDataProvider>());

            return controller;
        }
    }
}
