using System.Drawing;
using CarShop.ServiceDefaults.ServiceInterfaces.CarStorage;
using CarShop.Web.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Globalization;
using System.Text.RegularExpressions;
using CarShop.Web.Models.Catalog;

namespace CarShop.Web.Controllers
{
    [Route("[controller]")]
    public class CatalogController(CarStorageClient _carStorageClient) : Controller
    {
        private const int CARS_COUNT_ON_ONE_PAGE = 30;

        [HttpGet]
        [Route("{id:long?}")]
        public async Task<IActionResult> Index(
            [FromRoute] long? id,
            [FromQuery(Name = "brand")] string? brand,
            [FromQuery(Name = "minimum_engine_capacity")]
            double? minimumEngineCapacity,
            [FromQuery(Name = "maximum_engine_capacity")]
            double? maximumEngineCapacity,
            [FromQuery(Name = "fuel_type")] FuelType? fuelType,
            [FromQuery(Name = "corpus_type")] CorpusType? corpusType,
            [FromQuery(Name = "minimum_price")] double? minimumPrice,
            [FromQuery(Name = "maximum_price")] double? maximumPrice,
            [FromQuery(Name = "page")] int page = 1,
            [FromQuery(Name = "sort_by")] SortBy sortBy = SortBy.Brand,
            [FromQuery(Name = "sort_type")] SortType sortType = SortType.Ascending)
        {
            if (id is not null)
            {
                return await IdIndexAsync(id.Value);
            }

            if (page <= 0)
            {
                page = 1;
            }

            if (!Enum.IsDefined(sortBy))
                sortBy = SortBy.Brand;
            if (!Enum.IsDefined(sortType))
                sortType = SortType.Ascending;

            GetCarsOptions getCarsOptions = new GetCarsOptions
            {
                Brand = brand,
                MinimumEngineCapacity = minimumEngineCapacity,
                MaximumEngineCapacity = maximumEngineCapacity,
                FuelType = fuelType,
                CorpusType = corpusType,
                MinimumPrice = minimumPrice,
                MaximumPrice = maximumPrice,
                SortBy = sortBy,
                SortType = sortType,
            };

            GetCarsResult getCarsResult = await _carStorageClient.GetCarsAsync(getCarsOptions);

            IEnumerable<Car> cars = getCarsResult.Cars
                .Skip(CARS_COUNT_ON_ONE_PAGE * (page - 1))
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
                CurrentPage = page,
                PagesCount = pagesCount,
                IsSearchResultsPage = containsSearchParameters,
                GetCarsOptions = getCarsOptions,
            };
            return View(viewModel);
        }

        private async Task<IActionResult> IdIndexAsync(long id)
        {
            Car? car = await _carStorageClient.GetCarAsync(id);

            if (car is null)
            {
                return NotFound();
            }

            return View("~/Views/Catalog/id/Index.cshtml", car);
        }

        [HttpGet]
        [Route("{id:long}/configure")]
        public async Task<IActionResult> ConfigureAsync([FromRoute(Name = "id")] long id)
        {
            Car? car = await _carStorageClient.GetCarAsync(id);

            if (car is null)
            {
                return NotFound();
            }

            ConfigureViewModel viewModel = new()
            {
                Car = car
            };

            return View("~/Views/Catalog/id/Configure.cshtml", viewModel);
        }

        [HttpPost]
        [Route("{id:long}/configure")]
        public async Task<IActionResult> ConfigurePostAsync([FromRoute(Name = "id")] long id)
        {
            Car? car = await _carStorageClient.GetCarAsync(id);

            if (car is null)
            {
                return NotFound();
            }

            CarConfiguration carConfiguration = new()
            {
                CarId = car.Id,
                AirConditioner = Request.Form.ContainsKey("air_conditioner"),
                HeatedDriversSeat = Request.Form.ContainsKey("heated_drivers_seat"),
                SeatHeightAdjustment = Request.Form.ContainsKey("seat_height_adjustment"),
                DifferentCarColor =
                    Request.Form.TryGetValue("different_car_color", out var differentCarColor)
                        ? differentCarColor.First()!.ToLowerInvariant()
                        : null,
            };

            // Если не пришел ни один параметр
            if ((!carConfiguration.AirConditioner &&
                !carConfiguration.HeatedDriversSeat &&
                !carConfiguration.SeatHeightAdjustment &&
                carConfiguration.DifferentCarColor is null) || // или в поле цвета пришел не цвет
                (carConfiguration.DifferentCarColor is not null && 
                 !IsValidRgbHexColor(carConfiguration.DifferentCarColor)))
            {
                return BadRequest();
            }

            carConfiguration = await _carStorageClient.AddCarConfigurationAsync(carConfiguration);
            ConfigureViewModel viewModel = new()
            {
                Car = car,
                AddedCarConfiguration = carConfiguration,
            };
            return View("~/Views/Catalog/id/Configure.cshtml", viewModel);
        }
        
        private static bool IsValidRgbHexColor(string color)
        {
            string pattern = "^#([0-9a-fA-F]{6})$";
            return Regex.IsMatch(color, pattern);
        }
    }
}