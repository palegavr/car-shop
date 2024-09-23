using CarShop.ServiceDefaults.CommonTypes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
namespace CarShop.ServiceDefaults.ServiceInterfaces.CarStorage
{
    public class GetCarsResult
    {
        public IEnumerable<Car> Cars { get; init; }
        public long TotalResultsCount { get; init; }
    }
}