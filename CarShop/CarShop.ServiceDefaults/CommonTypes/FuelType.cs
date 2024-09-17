using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CarShop.ServiceDefaults.CommonTypes
{
    [Flags]
    public enum FuelType
    {
        Petrol = 1,
        Diesel = 2,
        Gas = 4,
        Electric = 8,
    }
}
