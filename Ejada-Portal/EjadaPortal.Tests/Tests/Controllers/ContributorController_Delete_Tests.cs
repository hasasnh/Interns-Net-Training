using Application.Services.IServices;
using EjadaPortal.Tests.Helpers;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc;
using Moq;
using Xunit;

namespace EjadaPortal.Tests.Tests.Controllers
{
    public class ContributorController_Delete_Tests
    {
        // POSITIVE (1)

        [Fact]
        public void Delete_ValidId_DeletesContributorAndReturnsJson()
        {
            // Arrange
            var contributorServiceMock = new Mock<IContributorService>();
            contributorServiceMock.Setup(s => s.DeleteContributor(1));

            var controller = ContributorControllerTestHelper.CreateControllerWithContext(contributorServiceMock);

            // Act
            var result = controller.Delete(1) as JsonResult;

            // Assert
            result.Should().NotBeNull();
            var jsonResult = result!.Value;
            jsonResult.Should().NotBeNull();
            
            // Verify the JSON response structure
            var responseProperties = jsonResult.GetType().GetProperties();
            responseProperties.Should().Contain(p => p.Name == "success");
            responseProperties.Should().Contain(p => p.Name == "message");
            
            controller.TempData["Success"].Should().Be("Contributor deleted successfully!");
            contributorServiceMock.Verify(s => s.DeleteContributor(1), Times.Once);
        }

        // NEGATIVE (2)

        [Fact]
        public void Delete_ServiceThrowsException_ReturnsJsonWithError()
        {
            // Arrange
            var contributorServiceMock = new Mock<IContributorService>();
            var exceptionMessage = "Database connection failed";
            contributorServiceMock.Setup(s => s.DeleteContributor(1))
                .Throws(new Exception(exceptionMessage));

            var controller = ContributorControllerTestHelper.CreateControllerWithContext(contributorServiceMock);

            // Act
            var result = controller.Delete(1) as JsonResult;

            // Assert
            result.Should().NotBeNull();
            var jsonResult = result!.Value;
            jsonResult.Should().NotBeNull();
            
            // Verify the JSON response structure
            var responseProperties = jsonResult.GetType().GetProperties();
            responseProperties.Should().Contain(p => p.Name == "success");
            responseProperties.Should().Contain(p => p.Name == "message");
            
            contributorServiceMock.Verify(s => s.DeleteContributor(1), Times.Once);
        }

        [Fact]
        public void Delete_InvalidId_HandlesGracefully()
        {
            // Arrange
            var contributorServiceMock = new Mock<IContributorService>();
            contributorServiceMock.Setup(s => s.DeleteContributor(999));

            var controller = ContributorControllerTestHelper.CreateControllerWithContext(contributorServiceMock);

            // Act
            var result = controller.Delete(999) as JsonResult;

            // Assert
            result.Should().NotBeNull();
            var jsonResult = result!.Value;
            jsonResult.Should().NotBeNull();
            
            // Verify the JSON response structure
            var responseProperties = jsonResult.GetType().GetProperties();
            responseProperties.Should().Contain(p => p.Name == "success");
            responseProperties.Should().Contain(p => p.Name == "message");
            
            controller.TempData["Success"].Should().Be("Contributor deleted successfully!");
            contributorServiceMock.Verify(s => s.DeleteContributor(999), Times.Once);
        }
    }
}
