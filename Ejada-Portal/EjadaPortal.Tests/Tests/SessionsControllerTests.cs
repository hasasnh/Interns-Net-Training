using Application.DTOs;
using Application.ServiceManager;
using Application.Services.IServices;
using Ejada_Portal.Controllers;
using Ejada_Portal.Web.Controllers;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Moq;
using System.Threading.Tasks;
using Xunit;

namespace EjadaPortal.Tests.Controllers
{
    public class SessionsControllerTests
    {
        private static SessionsController CreateControllerWith(Mock<IServiceManager> smMock, out Mock<ISessionService> sessionServiceMock)
        {
            sessionServiceMock = new Mock<ISessionService>();
            smMock.Setup(x => x.SessionService).Returns(sessionServiceMock.Object);

            var controller = new SessionsController(smMock.Object);

            var httpContext = new DefaultHttpContext();
            var tempDataProvider = new Mock<ITempDataProvider>().Object;
            controller.TempData = new TempDataDictionary(httpContext, tempDataProvider);

            return controller;
        }

        [Fact]
        public void Add_Get_ReturnsViewWithModel()
        {
            // Arrange
            var smMock = new Mock<IServiceManager>();
            var controller = CreateControllerWith(smMock, out _);

            // Act
            var result = controller.Add() as ViewResult;

            // Assert
            Assert.NotNull(result);
            Assert.IsType<SessionDto>(result.Model);
        }


        [Fact]
        public async Task Add_Post_InvalidModel_ReturnsViewAndDoesNotCallService()
        {
            // Arrange
            var smMock = new Mock<IServiceManager>();
            var controller = CreateControllerWith(smMock, out var sessionServiceMock);
            var badModel = new SessionDto { PresenterName = "", SessionName = "" };
            controller.ModelState.AddModelError("PresenterName", "Required");
            controller.ModelState.AddModelError("SessionName", "Required");

            // Act
            var result = await controller.Add(badModel);
            var view = Assert.IsType<ViewResult>(result);

            // Assert
            Assert.Same(badModel, view.Model);
            sessionServiceMock.Verify(s => s.CreateSessionAsync(It.IsAny<SessionDto>()), Times.Never);
        }
    }
}
