using CarShop.CarStorageService.Grpc;

namespace CarShop.Web.Models.Catalog;

public class ConfigurationViewModel
{
    public Car Car { get; set; }
    public CarConfiguration Configuration { get; set; }
}