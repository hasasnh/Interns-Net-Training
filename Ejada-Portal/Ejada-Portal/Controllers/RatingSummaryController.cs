using Application.DTOs;
using Application.ServiceManager;
using Microsoft.AspNetCore.Mvc;

namespace Ejada_Portal.Controllers
{
    public class RatingSummaryController : Controller
    {
        private readonly IServiceManager _serviceManager;

        public RatingSummaryController(IServiceManager serviceManager)
        {
            _serviceManager = serviceManager;
        }

        public async Task<IActionResult> RatingSummary()
        {
            var summary = await _serviceManager.SessionRatingService.GetSessionRatingsSummaryAsync();

            summary ??= new List<SessionRatingSummaryDto>();

            return View(summary);
        }
    }
}
