using CarShop.ServiceDefaults.CommonTypes;
using CarShop.ServiceDefaults.ServiceInterfaces.CarStorage;
using Microsoft.AspNetCore.Mvc;

namespace CarShop.Web.Controllers
{
    [Route("[controller]/{id?}")]
    public class CatalogController(CarStorageClient _carStorageClient) : Controller
    {
        [HttpGet]
        public async Task<IActionResult> Index([FromRoute] long? id)
        {
            if (id is not null) { return await IdIndexAsync(id.Value); }

            ViewData["Cars"] = (await _carStorageClient.GetCarsAsync()).Cars;
            return View();
        }

        [HttpPost]
        public IActionResult Index(
            [FromForm(Name = "brand")] string? brand,
            [FromForm(Name = "engine_capacity")] double engineCapacity,
            [FromForm(Name = "fuel_type")] FuelType fuelType,
            [FromForm(Name = "corpus_type")] CorpusType corpusType,
            [FromForm(Name = "minimal_price")] double minimalPrice,
            [FromForm(Name = "maximum_price")] double maximumPrice
            )
        {
            
            throw new NotImplementedException();
            return View();
        }

        public async Task<IActionResult> IdIndexAsync(long id)
        {
            Car? car = await _carStorageClient.GetCarAsync(id);

            if (car is null)
            {
                return NotFound();
            }

			return View("~/Views/Catalog/id/Index.cshtml", car);
        }
    }
}
