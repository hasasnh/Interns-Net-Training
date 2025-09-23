using Application.DTOs;
using Application.ServiceManager;
using Application.Services;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace WebApp.Controllers
{
    public class AssignRolesController : Controller
    {
        private readonly IServiceManager _serviceManager;

        public AssignRolesController(IServiceManager serviceManager)
        {
            _serviceManager = serviceManager;
        }

        public IActionResult Assign_Roles()
        {
            return View();
        }

        [HttpGet]
        public async Task<IActionResult> GetUsers()
        {
            var users = await _serviceManager.AssignRolesService.GetUsersAsync();
            return Json(new { data = users });
        }

        [HttpGet]
        public async Task<IActionResult> GetAllRoles()
        {
            var roles = await _serviceManager.AssignRolesService.GetAllRolesAsync();
            return Json(roles);
        }

        [HttpGet]
        public async Task<IActionResult> GetAllPages()
        {
            var pages = await _serviceManager.AssignRolesService.GetAllPagesAsync();
            return Json(pages);
        }




        [HttpPost]
        public async Task<IActionResult> SaveRoles([FromBody] List<UserRolesDto> usersRoles)
        {
            if (usersRoles == null || usersRoles.Count == 0)
                return BadRequest("No data received.");

            await _serviceManager.AssignRolesService.SaveRolesAsync(usersRoles);
            return Ok();
        }
    }
}
