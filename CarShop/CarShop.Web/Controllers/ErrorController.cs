using Microsoft.AspNetCore.Mvc;

namespace CarShop.Web.Controllers
{
	[Route("[controller]/{code}")]
	public class ErrorController : Controller
	{
		public IActionResult Index([FromRoute] int code)
		{
			if (code == StatusCodes.Status404NotFound)
				return NotFound404();
			else if (code == StatusCodes.Status401Unauthorized)
				return Redirect("/admin/login");

			return View();
		}

		private IActionResult NotFound404() 
		{
			return View("NotFound404");
		}
	}
}
