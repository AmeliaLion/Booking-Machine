using Microsoft.AspNetCore.Mvc;

namespace LawnMowingService.Controllers
{
    public class CustomerController : Controller
    {
        [HttpGet]
        public IActionResult Register()
        {
            return View();
        }

        [HttpGet]
        public IActionResult Index()
        {
            return View();
        }
    }
}