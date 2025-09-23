using Application.DTOs;
using Application.Services.IServices;
using Domain.Entities;
using Microsoft.AspNetCore.Identity;

public class AssignRolesService : IAssignRolesService
{
    private readonly UserManager<User> _userManager;
    private readonly RoleManager<IdentityRole> _roleManager;

    public AssignRolesService(UserManager<User> userManager, RoleManager<IdentityRole> roleManager)
    {
        _userManager = userManager;
        _roleManager = roleManager;
    }

    public async Task<IEnumerable<UserRolesDto>> GetUsersAsync()
    {
        var users = _userManager.Users.ToList();
        var allPages = await GetAllPagesAsync();

        var result = new List<UserRolesDto>();

        foreach (var user in users)
        {
            var roles = await _userManager.GetRolesAsync(user);

            result.Add(new UserRolesDto
            {
                Id = user.Id,
                Name = user.UserName,
                Email = user.Email,
                Role = roles.FirstOrDefault() ?? "",
                Permissions = allPages
                    .Where(p => !string.IsNullOrWhiteSpace(p))
                    .Select(p => p.Trim())
                    .Distinct()
                    .OrderBy(p => p)
                    .ToList()
            });
        }

        return result;
    }

    public async Task<IEnumerable<string>> GetAllRolesAsync()
    {
        return _roleManager.Roles.Select(r => r.Name).ToList();
    }

    public async Task<IEnumerable<string>> GetAllPagesAsync()
    {
        string viewsPath = Path.Combine(Directory.GetCurrentDirectory(), "Views");
        var pages = new List<string>();

        if (Directory.Exists(viewsPath))
        {
            var controllerFolders = Directory.GetDirectories(viewsPath);

            foreach (var folder in controllerFolders)
            {
                var viewFiles = Directory.GetFiles(folder, "*.cshtml", SearchOption.AllDirectories);
                foreach (var file in viewFiles)
                {
                    string pageName = Path.GetFileNameWithoutExtension(file);
                    if (!pageName.StartsWith("_")) // Without partials
                        pages.Add(pageName);
                }
            }
        }

        return pages.Distinct().OrderBy(p => p); 
    }

    public async Task SaveRolesAsync(List<UserRolesDto> usersRoles)
    {
        foreach (var userRole in usersRoles)
        {
            var user = await _userManager.FindByIdAsync(userRole.Id);
            if (user != null)
            {
                var existingRoles = await _userManager.GetRolesAsync(user);
                if (existingRoles.Any())
                    await _userManager.RemoveFromRolesAsync(user, existingRoles);

                if (!string.IsNullOrEmpty(userRole.Role))
                    await _userManager.AddToRoleAsync(user, userRole.Role);
            }
        }
    }
}
