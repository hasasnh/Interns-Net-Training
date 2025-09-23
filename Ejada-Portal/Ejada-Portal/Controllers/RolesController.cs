using Ejada_Portal.Controllers.Ejada_Portal.ViewModels;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System.Linq;
using System.Threading.Tasks;

namespace Ejada_Portal.Controllers
{
    public class RolesController : Controller
    {
        private readonly RoleManager<IdentityRole> _roleManager;

        public RolesController(RoleManager<IdentityRole> roleManager)
        {
            _roleManager = roleManager;
        }

        //  CreateRoles
        public IActionResult AddRoles()
        {
            return View();
        }

        // GET ALL ROLES
        [HttpGet]
        public IActionResult GetRoles()
        {
            var roles = _roleManager.Roles
                .Select(r => new UserRoleViewModel
                {
                    Id = r.Id,
                    Name = r.Name
                }).ToList();

            return Json(new { data = roles });
        }

        // Add new Roles
        [HttpPost]
        public async Task<IActionResult> AddRole([FromBody] UserRoleViewModel model)
        {
            if (!string.IsNullOrEmpty(model.Name) && !await _roleManager.RoleExistsAsync(model.Name))
            {
                await _roleManager.CreateAsync(new IdentityRole(model.Name));
                return Ok();
            }
            return BadRequest("Role already exists or invalid name.");
        }

        // Delete Role
        [HttpPost]
        public async Task<IActionResult> DeleteRole([FromBody] UserRoleViewModel model)
        {
            var role = await _roleManager.FindByIdAsync(model.Id);
            if (role != null)
            {
                await _roleManager.DeleteAsync(role);
                return Ok();
            }
            return NotFound();
        }

        //add roles to user 

        //  Assign Roles
        public IActionResult AssignRoles()
        {
            return View();
        }



    }
}
