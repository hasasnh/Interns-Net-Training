using Application.Services;
using Application.Services.IServices;
using Domain.Entities;
using Microsoft.AspNetCore.Identity;
using Moq;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Xunit;

public class AssignRolesServiceTests
{
    private readonly Mock<UserManager<User>> _userManagerMock;
    private readonly Mock<RoleManager<IdentityRole>> _roleManagerMock;
    private readonly IAssignRolesService _service;

    public AssignRolesServiceTests()
    {
        // Mock UserManager
        var userStore = new Mock<IUserStore<User>>();
        _userManagerMock = new Mock<UserManager<User>>(
            userStore.Object, null, null, null, null, null, null, null, null
        );

        // Mock RoleManager
        var roleStore = new Mock<IRoleStore<IdentityRole>>();
        _roleManagerMock = new Mock<RoleManager<IdentityRole>>(
            roleStore.Object, null, null, null, null
        );

        _service = new AssignRolesService(_userManagerMock.Object, _roleManagerMock.Object);
    }

    [Fact]
    public async Task GetAllRolesAsync_ShouldReturnRoles()
    {
        // Arrange
        var roles = new List<IdentityRole>
        {
            new IdentityRole("Admin"),
            new IdentityRole("User")
        }.AsQueryable();

        _roleManagerMock.Setup(r => r.Roles).Returns(roles);

        // Act
        var result = await _service.GetAllRolesAsync();

        // Assert
        Assert.NotNull(result);
        Assert.Contains("Admin", result);
        Assert.Contains("User", result);
    }
}
