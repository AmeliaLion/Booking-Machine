using Microsoft.AspNetCore.Mvc;

public class HomeController : Controller
{
    public IActionResult Error(int code)
    {
        if (code == 403) // Forbidden
        {
            return View("AccessDenied");
        }
        return View("Error");
    }
}