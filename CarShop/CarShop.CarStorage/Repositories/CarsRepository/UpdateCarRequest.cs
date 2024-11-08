using CarShop.CarStorage.Database.Entities.AdditionalCarOption;
using CarShop.CarStorage.Database.Entities.Car;

namespace CarShop.CarStorage.Repositories.CarsRepository
{
    public class UpdateCarRequest
    {
        public string? Brand { get; set; }
        public string? Model { get; set; }
        public double? PriceForStandartConfiguration { get; set; }
        public string? Color { get; set; }
        public double? EngineCapacity { get; set; }
        public CorpusType? CorpusType { get; set; }
        public FuelType? FuelType { get; set; }
        public int? Count { get; set; }
        public string? ImageUrl { get; set; }
        public string[]? BigImageURLs { get; set; }
        public AdditionalCarOption[]? AdditionalCarOptions { get; set; }
    }
}
