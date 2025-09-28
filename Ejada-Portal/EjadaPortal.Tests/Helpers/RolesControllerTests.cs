using Ejada_Portal.Controllers;
using Ejada_Portal.Controllers.Ejada_Portal.ViewModels; // <- namespace الصحيح لـ UserRoleViewModel
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Moq;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Xunit;

public class RolesControllerTests
{
    private readonly Mock<RoleManager<IdentityRole>> _mockRoleManager;
    private readonly RolesController _controller;

    public RolesControllerTests()
    {
        var store = new Mock<IRoleStore<IdentityRole>>();
        _mockRoleManager = new Mock<RoleManager<IdentityRole>>(store.Object, null, null, null, null);
        _controller = new RolesController(_mockRoleManager.Object);
    }

    [Fact]
    public void GetRoles_ShouldReturnRoles()
    {
        // Arrange
        var roles = new List<IdentityRole>
    {
        new IdentityRole { Id = "1", Name = "Admin" },
        new IdentityRole { Id = "2", Name = "User" }
    }.AsQueryable();

        _mockRoleManager.Setup(r => r.Roles).Returns(roles);

        // Act
        var result = _controller.GetRoles();

        // Assert
        var jsonResult = Assert.IsType<JsonResult>(result);

        // الوصول للخاصية 'data' من anonymous object
        var value = jsonResult.Value;
        var rolesDataProperty = value.GetType().GetProperty("data");
        var rolesData = rolesDataProperty.GetValue(value) as IEnumerable<UserRoleViewModel>;

        Assert.NotNull(rolesData);
        Assert.Equal(2, rolesData.Count());
        Assert.Contains(rolesData, r => r.Name == "Admin");
        Assert.Contains(rolesData, r => r.Name == "User");
    }



    [Fact]
    public async Task AddRole_ShouldAddNewRole_WhenRoleDoesNotExist()
    {
        // Arrange
        var model = new UserRoleViewModel { Name = "Manager" };
        _mockRoleManager.Setup(r => r.RoleExistsAsync(model.Name)).ReturnsAsync(false);
        _mockRoleManager.Setup(r => r.CreateAsync(It.IsAny<IdentityRole>())).ReturnsAsync(IdentityResult.Success);

        // Act
        var result = await _controller.AddRole(model);

        // Assert
        Assert.IsType<OkResult>(result);
        _mockRoleManager.Verify(r => r.CreateAsync(It.Is<IdentityRole>(role => role.Name == "Manager")), Times.Once);
    }

    [Fact]
    public async Task AddRole_ShouldReturnBadRequest_WhenRoleExists()
    {
        // Arrange
        var model = new UserRoleViewModel { Name = "Admin" };
        _mockRoleManager.Setup(r => r.RoleExistsAsync(model.Name)).ReturnsAsync(true);

        // Act
        var result = await _controller.AddRole(model);

        // Assert
        var badRequest = Assert.IsType<BadRequestObjectResult>(result);
        Assert.Equal("Role already exists or invalid name.", badRequest.Value);
    }
}
