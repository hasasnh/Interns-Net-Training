// Web/Controllers/SessionsController.cs
using Application.DTOs;
using Application.ServiceManager;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;

namespace Ejada_Portal.Web.Controllers
{
    public class SessionsController : Controller
    {
        private readonly IServiceManager _serviceManager;

        public SessionsController(IServiceManager serviceManager)
        {
            _serviceManager = serviceManager;
        }

        [HttpGet]
        public IActionResult Add()
        {
            return View(new SessionDto());
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Add(SessionDto dto)
        {
            if (!ModelState.IsValid)
                return View(dto);

            await _serviceManager.SessionService.CreateSessionAsync(dto);

            TempData["success"] = "Session created successfully!";
            return RedirectToAction(nameof(Add)); // الآن تبقى على صفحة Add
        }
    }
}
