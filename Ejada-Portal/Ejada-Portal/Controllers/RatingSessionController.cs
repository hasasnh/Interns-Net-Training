using Application.DTOs;
using Application.ServiceManager;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using System.Threading.Tasks;
using System.Linq;

namespace Ejada_Portal.Controllers
{
    public class RatingSessionController : Controller
    {
        private readonly IServiceManager _serviceManager;

        public RatingSessionController(IServiceManager serviceManager)
        {
            _serviceManager = serviceManager;
        }

        [HttpGet]
        public async Task<IActionResult> RatingSession()
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var allSessions = await _serviceManager.SessionService.GetAllSessionsAsync();

            if (!string.IsNullOrEmpty(userId))
            {
                var rated = await _serviceManager.SessionRatingService.GetRatingsByUserAsync(userId);
                allSessions = allSessions.Where(s => !rated.Any(r => r.SessionId == s.Id));
            }

            return View(allSessions);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Rate(int SessionId, int PresenterRate, int SessionRate, string Comment)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (string.IsNullOrEmpty(userId))
            {
                TempData["error"] = "You must be logged in to rate a session.";
                return RedirectToAction(nameof(RatingSession));
            }

            var dto = new SessionRatingDto
            {
                SessionId = SessionId,
                RatePresenter = PresenterRate,
                RateSession = SessionRate,
                Comments = Comment,
                UserId = userId
            };

            await _serviceManager.SessionRatingService.AddRatingAsync(dto);

            TempData["success"] = "Your rating has been saved!";
            return RedirectToAction(nameof(RatingSession));
        }
    }
}
