using Application.DTOs;
using Application.ServiceManager;
using Application.Services.IServices;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc;
using Moq;
using System.Collections.Generic;
using System.Threading.Tasks;
using WebApp.Controllers;
using Xunit;

namespace EjadaPortal.Tests.Controllers
{
    public class AssignRolesControllerTests
    {
        private readonly Mock<IServiceManager> _serviceManagerMock;
        private readonly Mock<IAssignRolesService> _assignRolesServiceMock;
        private readonly AssignRolesController _controller;

        public AssignRolesControllerTests()
        {
            _assignRolesServiceMock = new Mock<IAssignRolesService>();
            _serviceManagerMock = new Mock<IServiceManager>();
            _serviceManagerMock.Setup(sm => sm.AssignRolesService).Returns(_assignRolesServiceMock.Object);

            _controller = new AssignRolesController(_serviceManagerMock.Object);
        }

        [Fact]
        public void Assign_Roles_ShouldReturnView()
        {
            var result = _controller.Assign_Roles();
            result.Should().BeOfType<ViewResult>();
        }

       

        [Fact]
        public async Task GetAllRoles_ShouldReturnJsonWithRoles()
        {
            // Arrange
            var roles = new List<string> { "Admin", "User" };
            _assignRolesServiceMock.Setup(s => s.GetAllRolesAsync()).ReturnsAsync(roles);

            // Act
            var result = await _controller.GetAllRoles();

            // Assert
            var jsonResult = result as JsonResult;
            jsonResult.Should().NotBeNull();
            ((IEnumerable<string>)jsonResult!.Value).Should().BeEquivalentTo(roles);
        }

        [Fact]
        public async Task GetAllPages_ShouldReturnJsonWithPages()
        {
            // Arrange
            var pages = new List<string> { "Home", "Dashboard" };
            _assignRolesServiceMock.Setup(s => s.GetAllPagesAsync()).ReturnsAsync(pages);

            // Act
            var result = await _controller.GetAllPages();

            // Assert
            var jsonResult = result as JsonResult;
            jsonResult.Should().NotBeNull();
            ((IEnumerable<string>)jsonResult!.Value).Should().BeEquivalentTo(pages);
        }

    
        

        [Fact]
        public async Task SaveRoles_WithEmptyData_ShouldReturnBadRequest()
        {
            // Act
            var result = await _controller.SaveRoles(new List<UserRolesDto>());

            // Assert
            var badRequest = result as BadRequestObjectResult;
            badRequest.Should().NotBeNull();
            badRequest!.Value.Should().Be("No data received.");
            _assignRolesServiceMock.Verify(s => s.SaveRolesAsync(It.IsAny<List<UserRolesDto>>()), Times.Never);
        }
    }
}
