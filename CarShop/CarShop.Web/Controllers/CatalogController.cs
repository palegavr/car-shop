using CarShop.ServiceDefaults.CommonTypes;
using CarShop.ServiceDefaults.ServiceInterfaces.CarStorage;
using CarShop.Web.Models;
using Microsoft.AspNetCore.Mvc;
using System.Globalization;

namespace CarShop.Web.Controllers
{
    [Route("[controller]/{id?}")]
    public class CatalogController(CarStorageClient _carStorageClient) : Controller
    {
        [HttpGet]
        public async Task<IActionResult> Index([FromRoute] long? id)
        {
            if (id is not null) { return await IdIndexAsync(id.Value); }

            CatalogViewModel viewModel = new CatalogViewModel 
            { 
                Cars = (await _carStorageClient.GetCarsAsync()).Cars 
            };
            return View(viewModel);
        }

        [HttpPost]
        public async Task<IActionResult> Index(
			[FromForm(Name = "start_index")] int? startIndex,
			[FromForm(Name = "end_index")] int? endIndex,
			[FromForm(Name = "brand")] string? brand,
            [FromForm(Name = "minimum_engine_capacity")] double? minimumEngineCapacity,
            [FromForm(Name = "maximum_engine_capacity")] double? maximumEngineCapacity,
            [FromForm(Name = "fuel_type")] FuelType? fuelType,
            [FromForm(Name = "corpus_type")] CorpusType? corpus_type,
            [FromForm(Name = "minimum_price")] double? minimumPrice,
            [FromForm(Name = "maximum_price")] double? maximumPrice,
            [FromForm(Name = "sort_by")] SortBy? sortBy,
            [FromForm(Name = "sort_type")] SortType? sortType
			)
        {
			GetCarsOptions getCarsOptions = new GetCarsOptions
            {
                StartIndex = startIndex,
                EndIndex = endIndex,
                Brand = brand,
                MinimumEngineCapacity = minimumEngineCapacity,
                MaximumEngineCapacity = maximumEngineCapacity,
                FuelType = fuelType,
                CorpusType = corpus_type,
                MinimumPrice = minimumPrice,
                MaximumPrice = maximumPrice,
                SortBy = sortBy,
                SortType = sortType
            };

			CatalogViewModel viewModel = new CatalogViewModel
			{
				Cars = (await _carStorageClient.GetCarsAsync(getCarsOptions)).Cars,
                IsSearchResultsPage = true
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
