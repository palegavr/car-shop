using Microsoft.AspNetCore.Mvc;

namespace CarShop.Web.Controllers;

[Route("[controller]")]
public class AboutController : Controller
{
    [HttpGet]
    public IActionResult Index()
    {
        return View();
    }
}