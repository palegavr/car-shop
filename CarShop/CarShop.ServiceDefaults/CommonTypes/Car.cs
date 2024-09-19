using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CarShop.ServiceDefaults.CommonTypes
{
    public class Car
    {
        public const double SALE_TAX = 20.0;
        public long Id { get; set; }
        public string Brand { get; set; }
        public string Model { get; set; }
        public double PriceForStandardConfiguration { get; set; }
        public string Color { get; set; }
        public double EngineCapacity { get; set; }
        public CorpusType CorpusType { get; set; }
        public FuelType FuelType { get; set; }
        public int Count { get; set; }
        public string ImageUrl { get; set; }

        public double PriceForStandardConfigurationWithTax => PriceForStandardConfiguration + TaxForPriceForStandardConfiguration;
        public double TaxForPriceForStandardConfiguration => PriceForStandardConfiguration * (SALE_TAX / 100);
    }
}
