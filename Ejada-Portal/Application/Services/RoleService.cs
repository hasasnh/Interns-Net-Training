using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Services
{
    public class RoleService
    {
        private readonly RoleManager<IdentityRole> _roleManager;
        public RoleService(RoleManager<IdentityRole> roleManager)
        {
            _roleManager = roleManager;
        }
        public async Task<bool> RoleExistsAsync(string roleName)
        {
            return await _roleManager.RoleExistsAsync(roleName);
        }
        public async Task<IdentityResult> CreateRoleAsync(string roleName)
        {
            if (await RoleExistsAsync(roleName))
            {
                return IdentityResult.Failed(new IdentityError { Description = $"Role '{roleName}' already exists." });
            }
            var role = new IdentityRole(roleName);
            return await _roleManager.CreateAsync(role);
        }

        public
            async Task<IdentityResult> DeleteRoleAsync(string roleName)
        {
            var role = await _roleManager.FindByNameAsync(roleName);
            if (role == null)
            {
                return IdentityResult.Failed(new IdentityError { Description = $"Role '{roleName}' does not exist." });
            }
            return await _roleManager.DeleteAsync(role);
        }
        public async Task<IList<string>> GetAllRolesAsync()
        {
            return await Task.FromResult(_roleManager.Roles.Select(r => r.Name).ToList());
        }
        public async Task<IdentityRole> GetRoleByNameAsync(string roleName)
        {
            return await _roleManager.FindByNameAsync(roleName);
        }
       
      
        public async Task<IdentityResult> UpdateRoleNameAsync(string currentRoleName, string newRoleName)
        {
            var role = await _roleManager.FindByNameAsync(currentRoleName);
            if (role == null)
            {
                return IdentityResult.Failed(new IdentityError { Description = $"Role '{currentRoleName}' does not exist." });
            }
            if (await RoleExistsAsync(newRoleName))
            {
                return IdentityResult.Failed(new IdentityError { Description = $"Role '{newRoleName}' already exists." });
            }
            role.Name = newRoleName;
            role.NormalizedName = newRoleName.ToUpper();
            return await _roleManager.UpdateAsync(role);
        }

    }
}
