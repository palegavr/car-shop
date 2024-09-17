using CarShop.ServiceDefaults.CommonTypes;
using Microsoft.AspNetCore.Mvc;

namespace CarShop.Web.Controllers
{
    public class CatalogController : Controller
    {
        [HttpGet]
        public IActionResult Index()
        {
            throw new NotImplementedException();
            return View();
        }

        [HttpPost]
        public IActionResult Index(
            [FromForm(Name = "brand")] string? brand,
            [FromForm(Name = "engine_capacity")] double engineCapacity,
            [FromForm(Name = "fuel_type")] FuelType fuelType,
            [FromForm(Name = "corpus_type")] string? corpusType,
            [FromForm(Name = "minimal_price")] double minimalPrice,
            [FromForm(Name = "maximum_price")] double maximumPrice
            )
        {
            
            throw new NotImplementedException();
            return View();
        }
    }
}
