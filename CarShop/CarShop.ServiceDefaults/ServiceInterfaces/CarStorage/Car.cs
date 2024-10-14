using System.Text.Json.Serialization;

namespace CarShop.ServiceDefaults.ServiceInterfaces.CarStorage
{
    public class Car
    {
        public const double SALE_TAX = 20.0;
        public long Id { get; set; }
        public string Brand { get; set; }
        public string Model { get; set; }
        public double PriceForStandartConfiguration { get; set; }
        public string Color { get; set; }
        public double EngineCapacity { get; set; }
        public CorpusType CorpusType { get; set; }
        public FuelType FuelType { get; set; }
        public int Count { get; set; }
        public string ImageUrl { get; set; }
        public string[] BigImageURLs { get; set; } = [];
        public List<AdditionalCarOption> AdditionalCarOptions { get; set; } = new();

        [JsonIgnore] public double PriceForStandardConfigurationWithTax => PriceForStandartConfiguration + TaxForPriceForStandardConfiguration;
        [JsonIgnore] public double TaxForPriceForStandardConfiguration => PriceForStandartConfiguration * (SALE_TAX / 100);
    }
}
