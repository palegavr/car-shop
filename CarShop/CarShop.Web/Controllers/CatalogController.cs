using CarShop.ServiceDefaults.CommonTypes;
using CarShop.ServiceDefaults.ServiceInterfaces.CarStorage;
using CarShop.Web.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Globalization;

namespace CarShop.Web.Controllers
{
    [Route("[controller]/{id:long?}")]
    public class CatalogController(CarStorageClient _carStorageClient) : Controller
    {
        private const int CARS_COUNT_ON_ONE_PAGE = 30;

        [HttpGet]
        public async Task<IActionResult> Index(
            [FromRoute] long? id,
			[FromQuery(Name = "brand")] string? brand,
			[FromQuery(Name = "minimum_engine_capacity")] double? minimumEngineCapacity,
			[FromQuery(Name = "maximum_engine_capacity")] double? maximumEngineCapacity,
			[FromQuery(Name = "fuel_type")] FuelType? fuelType,
			[FromQuery(Name = "corpus_type")] CorpusType? corpusType,
			[FromQuery(Name = "minimum_price")] double? minimumPrice,
			[FromQuery(Name = "maximum_price")] double? maximumPrice,
			[FromQuery(Name = "page")] int? page)
        {
            if (id is not null) { return await IdIndexAsync(id.Value); }
            
            page ??= 1;
            if (page <= 0) 
                page = 1;

			GetCarsOptions getCarsOptions = new GetCarsOptions
			{
				Brand = brand,
				MinimumEngineCapacity = minimumEngineCapacity,
				MaximumEngineCapacity = maximumEngineCapacity,
				FuelType = fuelType,
				CorpusType = corpusType,
				MinimumPrice = minimumPrice,
				MaximumPrice = maximumPrice
			};

			GetCarsResult getCarsResult = await _carStorageClient.GetCarsAsync(getCarsOptions);

            IEnumerable<Car> cars = getCarsResult.Cars
                .Skip(CARS_COUNT_ON_ONE_PAGE * (page.Value - 1))
                .Take(CARS_COUNT_ON_ONE_PAGE);

            int pagesCount = getCarsResult.TotalResultsCount / CARS_COUNT_ON_ONE_PAGE;
            if (getCarsResult.TotalResultsCount % CARS_COUNT_ON_ONE_PAGE > 0)
                pagesCount++;

            bool containsSearchParameters =
                brand is not null ||
                minimumEngineCapacity is not null ||
                maximumEngineCapacity is not null ||
                fuelType is not null ||
                corpusType is not null ||
                minimumPrice is not null ||
                maximumPrice is not null;

            CatalogViewModel viewModel = new CatalogViewModel
            {
                Cars = cars,
                CurrentPage = page.Value,
                PagesCount = pagesCount,
                IsSearchResultsPage = containsSearchParameters
            };
            return View(viewModel);
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
