using Application.DTOs;
using Application.Services.IServices;
using EjadaPortal.Tests.Helpers;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc;
using Moq;
using Xunit;

namespace EjadaPortal.Tests.Tests.Controllers
{
    public class ContributorController_Details_Tests
    {
        // POSITIVE (1)

        [Fact]
        public void Details_ReturnsView_WhenContributorExists()
        {
            // Arrange
            var contributorServiceMock = new Mock<IContributorService>();
            var contributor = new ContributorDTO
            {
                Id = 1,
                Name = "John Doe",
                Email = "john@example.com",
                Description = "Senior Developer",
                Role = "Developer",
                LinkedInUrl = "https://linkedin.com/in/johndoe"
            };
            contributorServiceMock.Setup(s => s.GetContributorById(1)).Returns(contributor);

            var controller = ContributorControllerTestHelper.CreateControllerWithContext(contributorServiceMock);

            // Act
            var result = controller.Details(1) as ViewResult;

            // Assert
            result.Should().NotBeNull();
            var model = result!.Model as ContributorDTO;
            model.Should().NotBeNull();
            model!.Id.Should().Be(1);
            model.Name.Should().Be("John Doe");
            model.Email.Should().Be("john@example.com");
            contributorServiceMock.Verify(s => s.GetContributorById(1), Times.Once);
        }

        // NEGATIVE (1)

        [Fact]
        public void Details_ReturnsNotFound_WhenContributorDoesNotExist()
        {
            // Arrange
            var contributorServiceMock = new Mock<IContributorService>();
            contributorServiceMock.Setup(s => s.GetContributorById(999)).Returns((ContributorDTO?)null);

            var controller = ContributorControllerTestHelper.CreateControllerWithContext(contributorServiceMock);

            // Act
            var result = controller.Details(999);

            // Assert
            result.Should().BeOfType<NotFoundResult>();
            contributorServiceMock.Verify(s => s.GetContributorById(999), Times.Once);
        }
    }
}
