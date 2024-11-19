using System.Drawing;
using CarShop.Web.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Globalization;
using System.Text.RegularExpressions;
using CarShop.CarStorageService.Grpc;
using CarShop.Web.Models.Catalog;

namespace CarShop.Web.Controllers
{
    [Route("[controller]")]
    public class CatalogController(
        CarStorageService.Grpc.CarStorageService.CarStorageServiceClient _carStorageClient) : Controller
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
            [FromQuery(Name = "fuel_type")] Car.Types.FuelType? fuelType,
            [FromQuery(Name = "corpus_type")] Car.Types.CorpusType? corpusType,
            [FromQuery(Name = "minimum_price")] double? minimumPrice,
            [FromQuery(Name = "maximum_price")] double? maximumPrice,
            [FromQuery(Name = "page")] int page = 1,
            [FromQuery(Name = "sort_by")] GetCarsRequest.Types.SortBy sortBy = GetCarsRequest.Types.SortBy.Brand,
            [FromQuery(Name = "sort_type")] GetCarsRequest.Types.SortType sortType = GetCarsRequest.Types.SortType.Ascending)
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
                sortBy = GetCarsRequest.Types.SortBy.Brand;
            if (!Enum.IsDefined(sortType))
                sortType = GetCarsRequest.Types.SortType.Ascending;

            var getCarsRequest = new GetCarsRequest
            {
                SortBy = sortBy,
                SortType = sortType
            };

            if (brand is not null)
            {
                getCarsRequest.Brand = brand;
            }
            if (minimumEngineCapacity is not null)
            {
                getCarsRequest.MinimumEngineCapacity = minimumEngineCapacity.Value;
            }
            if (maximumEngineCapacity is not null)
            {
                getCarsRequest.MaximumEngineCapacity = maximumEngineCapacity.Value;
            }
            if (fuelType is not null)
            {
                getCarsRequest.FuelType = fuelType.Value;
            }
            if (corpusType is not null)
            {
                getCarsRequest.CorpusType = corpusType.Value;
            }
            if (minimumPrice is not null)
            {
                getCarsRequest.MinimumPrice = minimumPrice.Value;
            }
            if (maximumPrice is not null)
            {
                getCarsRequest.MaximumPrice = maximumPrice.Value;
            }

            var getCarsReply = await _carStorageClient.GetCarsAsync(getCarsRequest);

            IEnumerable<Car> cars = getCarsReply.Cars
                .Skip(CARS_COUNT_ON_ONE_PAGE * (page - 1))
                .Take(CARS_COUNT_ON_ONE_PAGE);

            int pagesCount = getCarsReply.TotalResultsCount / CARS_COUNT_ON_ONE_PAGE;
            if (getCarsReply.TotalResultsCount % CARS_COUNT_ON_ONE_PAGE > 0)
            {
                pagesCount++;
            }

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
                GetCarsOptions = getCarsRequest,
            };
            return View(viewModel);
        }

        private async Task<IActionResult> IdIndexAsync(long id)
        {
            var getCarReply = await _carStorageClient.GetCarAsync(new()
            {
                CarId = id
            });
            
            if (getCarReply.Result == GetCarReply.Types.GetCarResult.CarNotFound)
            {
                return NotFound();
            }

            return View("~/Views/Catalog/id/Index.cshtml", getCarReply.Car);
        }

        [HttpGet]
        [Route("{id:long}/configure")]
        public async Task<IActionResult> ConfigureAsync([FromRoute(Name = "id")] long id)
        {
            var getCarReply = await _carStorageClient.GetCarAsync(new()
            {
                CarId = id
            });
            
            if (getCarReply.Result == GetCarReply.Types.GetCarResult.CarNotFound)
            {
                return NotFound();
            }

            ConfigureViewModel viewModel = new()
            {
                Car = getCarReply.Car
            };

            return View("~/Views/Catalog/id/Configure.cshtml", viewModel);
        }

        [HttpPost]
        [Route("{id:long}/configure")]
        public async Task<IActionResult> ConfigurePostAsync([FromRoute(Name = "id")] long id)
        {
            var getCarReply = await _carStorageClient.GetCarAsync(new()
            {
                CarId = id
            });
            
            if (getCarReply.Result == GetCarReply.Types.GetCarResult.CarNotFound)
            {
                return NotFound();
            }

            var car = getCarReply.Car;

            CarConfiguration carConfiguration = new()
            {
                CarId = car.Id,
                AirConditioner = Request.Form.ContainsKey("air_conditioner"),
                HeatedDriversSeat = Request.Form.ContainsKey("heated_drivers_seat"),
                SeatHeightAdjustment = Request.Form.ContainsKey("seat_height_adjustment"),
            };

            if (Request.Form.TryGetValue("different_car_color", out var differentCarColor))
            {
                carConfiguration.DifferentCarColor = differentCarColor.First()!.ToLowerInvariant();
            }

            var addCarConfigurationReply = await _carStorageClient.AddCarConfigurationAsync(new()
            {
                CarConfiguration = carConfiguration
            });

            if (addCarConfigurationReply.Result == AddCarConfigurationReply.Types.AddCarConfigurationResult
                    .CarConfigurationHaveUnavailableOptions)
            {
                return BadRequest();
            }
            
            if (addCarConfigurationReply.Result != AddCarConfigurationReply.Types.AddCarConfigurationResult.Success)
            {
                return Problem();
            }
            
            ConfigureViewModel viewModel = new()
            {
                Car = car,
                AddedCarConfiguration = addCarConfigurationReply.CarConfiguration,
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