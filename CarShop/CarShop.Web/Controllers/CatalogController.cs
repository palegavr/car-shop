using CarShop.ServiceDefaults.CommonTypes;
using Microsoft.AspNetCore.Mvc;

namespace CarShop.Web.Controllers
{
    [Route("{controller}/{id?}")]
    public class CatalogController : Controller
    {
        private List<Car> _cars = new List<Car>()
        {
            new Car { Id = 1, Brand = "Brand1", Model = "Model1",
                PriceForStandardConfiguration = 100,
                Color = "Красный",
                EngineCapacity = 30,
                CorpusType = CorpusType.Sedan,
                FuelType = FuelType.Petrol,
                Count = 123,
                ImageUrl = "https://img1.akspic.ru/previews/3/8/0/8/7/178083/178083-gonochnyj_avtomobil-legkovyye_avtomobili-avtomobilnoe_osveshhenie-koleso-purpurnyj_cvet-x750.jpg" },

            new Car { Id = 2, Brand = "Brand2", Model = "Model2",
                PriceForStandardConfiguration = 200,
                Color = "Желтый",
                EngineCapacity = 40,
                CorpusType = CorpusType.Sedan,
                FuelType = FuelType.Diesel,
                Count = 456,
                ImageUrl = "https://img2.akspic.ru/previews/2/8/1/8/7/178182/178182-legkovyye_avtomobili-sportkar-lambordzhini-superkar-lamborgini_aventador-x750.jpg" },
            new Car { Id = 3, Brand = "Brand3", Model = "Model3",
                PriceForStandardConfiguration = 300,
                Color = "Зеленый",
                EngineCapacity = 50,
                CorpusType = CorpusType.Hatchback,
                FuelType = FuelType.Diesel | FuelType.Electric,
                Count = 987,
                ImageUrl = "https://img3.akspic.ru/previews/1/7/8/7/7/177871/177871-zelenyj_lamborgini-lamborghini_gallardo-lambordzhini-lamborghini_urakan-legkovyye_avtomobili-x750.jpg" }
        };

        [HttpGet]
        public IActionResult Index()
        {
            ViewData["Cars"] = _cars;
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
    }
}
