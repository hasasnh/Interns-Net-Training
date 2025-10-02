using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Ejada_Portal.Controllers
{
    [Authorize]
    public class CalenderController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
