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
    public class ContributorController_Edit_Tests
    {
        // POSITIVE (3)

        [Fact]
        public void Edit_GET_ReturnsView_WhenContributorExists()
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
                Photo = "/uploads/contributors/john.jpg"
            };
            contributorServiceMock.Setup(s => s.GetContributorById(1)).Returns(contributor);

            var controller = ContributorControllerTestHelper.CreateControllerWithContext(contributorServiceMock);

            // Act
            var result = controller.Edit(1) as ViewResult;

            // Assert
            result.Should().NotBeNull();
            var model = result!.Model as ContributorDTO;
            model.Should().NotBeNull();
            model!.Id.Should().Be(1);
            model.Name.Should().Be("John Doe");
            model.Photo.Should().Be("/uploads/contributors/john.jpg");
            contributorServiceMock.Verify(s => s.GetContributorById(1), Times.Once);
        }

        [Fact]
        public async Task Edit_POST_ValidModel_WithoutPhoto_UpdatesContributorAndRedirects()
        {
            // Arrange
            var contributorServiceMock = new Mock<IContributorService>();
            var contributor = new ContributorDTO
            {
                Id = 1,
                Name = "John Doe Updated",
                Email = "john.updated@example.com",
                Description = "Lead Developer",
                Role = "Lead",
                Photo = "/uploads/contributors/existing.jpg"
            };
            contributorServiceMock.Setup(s => s.UpdateContributor(It.IsAny<ContributorDTO>()));

            var controller = ContributorControllerTestHelper.CreateControllerWithContext(contributorServiceMock);

            // Act
            var result = await controller.Edit(contributor, null) as RedirectToActionResult;

            // Assert
            result.Should().NotBeNull();
            result!.ActionName.Should().Be(nameof(ContributorController.Index));
            controller.TempData["Success"].Should().Be("Contributor updated successfully!");
            contributorServiceMock.Verify(s => s.UpdateContributor(contributor), Times.Once);
        }

        [Fact]
        public async Task Edit_POST_ValidModel_WithPhoto_UpdatesContributorWithNewPhotoAndRedirects()
        {
            // Arrange
            var contributorServiceMock = new Mock<IContributorService>();
            var contributor = new ContributorDTO
            {
                Id = 1,
                Name = "John Doe",
                Email = "john@example.com",
                Description = "Senior Developer",
                Role = "Developer"
            };
            contributorServiceMock.Setup(s => s.UpdateContributor(It.IsAny<ContributorDTO>()));

            var photoFile = new Mock<IFormFile>();
            photoFile.Setup(f => f.Length).Returns(1024);
            photoFile.Setup(f => f.FileName).Returns("newphoto.jpg");
            photoFile.Setup(f => f.CopyToAsync(It.IsAny<Stream>(), It.IsAny<CancellationToken>()))
                .Returns(Task.CompletedTask);

            var controller = ContributorControllerTestHelper.CreateControllerWithContext(contributorServiceMock);

            // Act
            var result = await controller.Edit(contributor, photoFile.Object) as RedirectToActionResult;

            // Assert
            result.Should().NotBeNull();
            result!.ActionName.Should().Be(nameof(ContributorController.Index));
            controller.TempData["Success"].Should().Be("Contributor updated successfully!");
            contributorServiceMock.Verify(s => s.UpdateContributor(It.Is<ContributorDTO>(c => c.Photo != null)), Times.Once);
        }

        // NEGATIVE (2)

        [Fact]
        public void Edit_GET_ReturnsNotFound_WhenContributorDoesNotExist()
        {
            // Arrange
            var contributorServiceMock = new Mock<IContributorService>();
            contributorServiceMock.Setup(s => s.GetContributorById(999)).Returns((ContributorDTO?)null);

            var controller = ContributorControllerTestHelper.CreateControllerWithContext(contributorServiceMock);

            // Act
            var result = controller.Edit(999);

            // Assert
            result.Should().BeOfType<NotFoundResult>();
            contributorServiceMock.Verify(s => s.GetContributorById(999), Times.Once);
        }

        [Fact]
        public async Task Edit_POST_InvalidModel_ReturnsView()
        {
            // Arrange
            var contributorServiceMock = new Mock<IContributorService>();
            var contributor = new ContributorDTO
            {
                Id = 1,
                Name = "", // Invalid - empty name
                Email = "invalid-email" // Invalid email format
            };

            var controller = ContributorControllerTestHelper.CreateControllerWithContext(contributorServiceMock);
            controller.ModelState.AddModelError("Name", "Name is required");

            // Act
            var result = await controller.Edit(contributor, null) as ViewResult;

            // Assert
            result.Should().NotBeNull();
            result!.Model.Should().BeEquivalentTo(contributor);
            result.ViewData.ModelState.IsValid.Should().BeFalse();
            contributorServiceMock.Verify(s => s.UpdateContributor(It.IsAny<ContributorDTO>()), Times.Never);
        }
    }
}
