using Application.DTOs;
using Application.ServiceManager;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;

namespace Ejada_Portal.Controllers
{
    [Authorize] 
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
        public async Task<IActionResult> Add(SessionDto model)
        {
            if (!ModelState.IsValid)
                return View(model);

            await _serviceManager.SessionService.AddAsync(model);
            TempData["success"] = "Session added successfully.";
            return RedirectToAction(nameof(RatingSession));
        }

        [HttpGet]
        public async Task<IActionResult> RatingSession()
        {
            var sessions = await _serviceManager.SessionService.GetAllAsync();
            return View(sessions);
        }
    }
}
