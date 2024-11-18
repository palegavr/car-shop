using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;
using CarShop.CarStorage.ValidationAttributes;
using CarShop.ServiceDefaults.Attributes.Validation;

namespace CarShop.CarStorage.Database.Entities.Car
{
    public class Car
    {
        public const double SALE_TAX = 20.0;
        public long Id { get; set; }
        [Required] public string Brand { get; set; }
        [Required] public string Model { get; set; }

        [Required]
        [Range(0.0, double.MaxValue, MinimumIsExclusive = true)]
        public double PriceForStandartConfiguration { get; set; }

        [Required] public string Color { get; set; }

        [Required]
        [Range(0.0, double.MaxValue)]
        [EngineCapacityFuelType]
        public double EngineCapacity { get; set; }

        [Required] [EnumValidation(typeof(CorpusType))] public CorpusType CorpusType { get; set; }
        [Required] [EnumValidation(typeof(FuelType))] public FuelType FuelType { get; set; }
        [Required] [Range(0, int.MaxValue)] public int Count { get; set; }
        public string ImageUrl { get; set; }
        [Required] public string[] BigImageURLs { get; set; } = [];

        [AdditionalCarOptions]
        public List<AdditionalCarOption.AdditionalCarOption> AdditionalCarOptions { get; set; } = new();

        public List<CarConfiguration> CarConfigurations { get; set; } = new();
        public List<CarEditProcess.CarEditProcess> CarEditProcesses { get; set; } = new();

        [JsonIgnore]
        public double PriceForStandardConfigurationWithTax =>
            PriceForStandartConfiguration + TaxForPriceForStandardConfiguration;

        [JsonIgnore]
        public double TaxForPriceForStandardConfiguration => PriceForStandartConfiguration * (SALE_TAX / 100);
    }
}