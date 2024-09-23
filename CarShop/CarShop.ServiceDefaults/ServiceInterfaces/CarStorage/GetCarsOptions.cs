using CarShop.ServiceDefaults.CommonTypes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
namespace CarShop.ServiceDefaults.ServiceInterfaces.CarStorage
{
    public class GetCarsOptions
    {
        public Range? FromTo { get; set; } = null;
        public string? Brand { get; set; } = null;
        public double? EngineCapacityFrom { get; set; } = null;
        public double? EngineCapacityTo { get; set; } = null;
        public FuelType? FuelType { get; set; } = null;
        public CorpusType? CorpusType { get; set; } = null;
        public double? PriceFrom { get; set; } = null;
        public double? PriceTo { get; set; } = null;
        public bool OrderByDesc { get; set; } = false;
    }
}