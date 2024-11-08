using CarShop.CarStorageService.Grpc;

namespace CarShop.Web.Models.Catalog;

public class ConfigureViewModel
{
    public Car Car { get; set; }
    public CarConfiguration? AddedCarConfiguration { get; set; }
}