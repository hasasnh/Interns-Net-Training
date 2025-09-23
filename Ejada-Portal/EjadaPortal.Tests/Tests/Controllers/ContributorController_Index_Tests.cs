using Application.DTOs;
using Application.Services.IServices;
using EjadaPortal.Tests.Helpers;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc;
using Moq;
using Xunit;

namespace EjadaPortal.Tests.Tests.Controllers
{
    public class ContributorController_Index_Tests
    {
        // POSITIVE (2)

        [Fact]
        public void Index_ReturnsView_WithAllContributors()
        {
            // Arrange
            var contributorServiceMock = new Mock<IContributorService>();
            var contributors = new List<ContributorDTO>
            {
                new ContributorDTO { Id = 1, Name = "John Doe", Email = "john@example.com" },
                new ContributorDTO { Id = 2, Name = "Jane Smith", Email = "jane@example.com" }
            };
            contributorServiceMock.Setup(s => s.GetAllContributors()).Returns(contributors);

            var controller = ContributorControllerTestHelper.CreateControllerWithContext(contributorServiceMock);

            // Act
            var result = controller.Index() as ViewResult;

            // Assert
            result.Should().NotBeNull();
            var model = result!.Model as List<ContributorDTO>;
            model.Should().NotBeNull();
            model!.Should().HaveCount(2);
            model.Should().Contain(c => c.Name == "John Doe");
            model.Should().Contain(c => c.Name == "Jane Smith");
            contributorServiceMock.Verify(s => s.GetAllContributors(), Times.Once);
        }

        [Fact]
        public void Index_ReturnsView_WithEmptyList_WhenNoContributors()
        {
            // Arrange
            var contributorServiceMock = new Mock<IContributorService>();
            var emptyContributors = new List<ContributorDTO>();
            contributorServiceMock.Setup(s => s.GetAllContributors()).Returns(emptyContributors);

            var controller = ContributorControllerTestHelper.CreateControllerWithContext(contributorServiceMock);

            // Act
            var result = controller.Index() as ViewResult;

            // Assert
            result.Should().NotBeNull();
            var model = result!.Model as List<ContributorDTO>;
            model.Should().NotBeNull();
            model!.Should().BeEmpty();
            contributorServiceMock.Verify(s => s.GetAllContributors(), Times.Once);
        }
    }
}
