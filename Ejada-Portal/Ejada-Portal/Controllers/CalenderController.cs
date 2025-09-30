using Microsoft.AspNetCore.Mvc;

namespace Ejada_Portal.Controllers
{
    public class CalenderController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
