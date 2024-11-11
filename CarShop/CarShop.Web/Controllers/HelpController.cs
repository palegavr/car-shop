using Microsoft.AspNetCore.Mvc;

namespace CarShop.Web.Controllers;

[Route("[controller]")]
public class HelpController : Controller
{
    [HttpGet]
    public IActionResult Index()
    {
        return View();
    }
}