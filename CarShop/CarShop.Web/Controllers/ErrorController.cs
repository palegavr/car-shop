using Microsoft.AspNetCore.Mvc;

namespace CarShop.Web.Controllers
{
	[Route("[controller]/{code}")]
	public class ErrorController : Controller
	{
		public IActionResult Index([FromRoute] int code)
		{
			return code switch
			{
				StatusCodes.Status404NotFound => NotFound404(),
				StatusCodes.Status500InternalServerError => InternalError500(),
				StatusCodes.Status401Unauthorized => Unauthorized401(),
				StatusCodes.Status400BadRequest => BadRequest400(),
				_ => View(),
			};
		}

		private IActionResult NotFound404() 
		{
			return View("NotFound404");
		}
		
		private IActionResult Unauthorized401() 
		{
			return Redirect("/admin/login");
		}
		
		private IActionResult BadRequest400()
		{
			return View("BadRequest400");
		}

		private IActionResult InternalError500()
		{
			return View("InternalError500");
		}
	}
}
