using Application.DTOs;
using Application.Services.IServices;
using Ejada_Portal.Controllers;
using EjadaPortal.Tests.Helpers;
using FluentAssertions;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Moq;
using Xunit;

namespace EjadaPortal.Tests.Tests.Controllers
{
    public class ContributorController_Create_Tests
    {
        // POSITIVE (3)

        [Fact]
        public void Create_GET_ReturnsView()
        {
            // Arrange
            var contributorServiceMock = new Mock<IContributorService>();
            var controller = ContributorControllerTestHelper.CreateControllerWithContext(contributorServiceMock);

            // Act
            var result = controller.Create() as ViewResult;

            // Assert
            result.Should().NotBeNull();
            result!.Model.Should().BeNull();
        }

        [Fact]
        public async Task Create_POST_ValidModel_WithoutPhoto_CreatesContributorAndRedirects()
        {
            // Arrange
            var contributorServiceMock = new Mock<IContributorService>();
            var contributor = new ContributorDTO
            {
                Name = "John Doe",
                Email = "john@example.com",
                Description = "Senior Developer",
                Role = "Developer"
            };
            contributorServiceMock.Setup(s => s.CreateContributor(It.IsAny<ContributorDTO>()));

            var controller = ContributorControllerTestHelper.CreateControllerWithContext(contributorServiceMock);

            // Act
            var result = await controller.Create(contributor, null) as RedirectToActionResult;

            // Assert
            result.Should().NotBeNull();
            result!.ActionName.Should().Be(nameof(ContributorController.Index));
            controller.TempData["Success"].Should().Be("Contributor created successfully!");
            contributorServiceMock.Verify(s => s.CreateContributor(contributor), Times.Once);
        }

        [Fact]
        public async Task Create_POST_ValidModel_WithPhoto_CreatesContributorWithPhotoAndRedirects()
        {
            // Arrange
            var contributorServiceMock = new Mock<IContributorService>();
            var contributor = new ContributorDTO
            {
                Name = "John Doe",
                Email = "john@example.com",
                Description = "Senior Developer",
                Role = "Developer"
            };
            contributorServiceMock.Setup(s => s.CreateContributor(It.IsAny<ContributorDTO>()));

            var photoFile = new Mock<IFormFile>();
            photoFile.Setup(f => f.Length).Returns(1024);
            photoFile.Setup(f => f.FileName).Returns("test.jpg");
            photoFile.Setup(f => f.CopyToAsync(It.IsAny<Stream>(), It.IsAny<CancellationToken>()))
                .Returns(Task.CompletedTask);

            var controller = ContributorControllerTestHelper.CreateControllerWithContext(contributorServiceMock);

            // Act
            var result = await controller.Create(contributor, photoFile.Object) as RedirectToActionResult;

            // Assert
            result.Should().NotBeNull();
            result!.ActionName.Should().Be(nameof(ContributorController.Index));
            controller.TempData["Success"].Should().Be("Contributor created successfully!");
            contributorServiceMock.Verify(s => s.CreateContributor(It.Is<ContributorDTO>(c => c.Photo != null)), Times.Once);
        }

        // NEGATIVE (2)

        [Fact]
        public async Task Create_POST_InvalidModel_ReturnsView()
        {
            // Arrange
            var contributorServiceMock = new Mock<IContributorService>();
            var contributor = new ContributorDTO
            {
                Name = "", // Invalid - empty name
                Email = "invalid-email" // Invalid email format
            };

            var controller = ContributorControllerTestHelper.CreateControllerWithContext(contributorServiceMock);
            controller.ModelState.AddModelError("Name", "Name is required");

            // Act
            var result = await controller.Create(contributor, null) as ViewResult;

            // Assert
            result.Should().NotBeNull();
            result!.Model.Should().BeEquivalentTo(contributor);
            result.ViewData.ModelState.IsValid.Should().BeFalse();
            contributorServiceMock.Verify(s => s.CreateContributor(It.IsAny<ContributorDTO>()), Times.Never);
        }

        [Fact]
        public async Task Create_POST_EmptyPhotoFile_DoesNotUpdatePhoto()
        {
            // Arrange
            var contributorServiceMock = new Mock<IContributorService>();
            var contributor = new ContributorDTO
            {
                Name = "John Doe",
                Email = "john@example.com",
                Description = "Senior Developer",
                Role = "Developer"
            };
            contributorServiceMock.Setup(s => s.CreateContributor(It.IsAny<ContributorDTO>()));

            var emptyPhotoFile = new Mock<IFormFile>();
            emptyPhotoFile.Setup(f => f.Length).Returns(0); // Empty file

            var controller = ContributorControllerTestHelper.CreateControllerWithContext(contributorServiceMock);

            // Act
            var result = await controller.Create(contributor, emptyPhotoFile.Object) as RedirectToActionResult;

            // Assert
            result.Should().NotBeNull();
            result!.ActionName.Should().Be(nameof(ContributorController.Index));
            contributorServiceMock.Verify(s => s.CreateContributor(contributor), Times.Once);
        }
    }
}
