using CarShop.ServiceDefaults.ServiceInterfaces.CarStorage;

namespace CarShop.Web.Models.Catalog;

public class ConfigureViewModel
{
    public Car Car { get; set; }
    public CarConfiguration? AddedCarConfiguration { get; set; }
}