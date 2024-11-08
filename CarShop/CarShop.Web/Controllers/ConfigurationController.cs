using CarShop.CarStorageService.Grpc;
using CarShop.Web.Models.Catalog;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;

namespace CarShop.Web.Controllers;

[Route("[controller]")]
public class ConfigurationController(
    CarStorageService.Grpc.CarStorageService.CarStorageServiceClient _carStorageServiceClient) : Controller
{

    [HttpGet]
    [Route("{id:guid}")]
    public async Task<IActionResult> IdIndexAsync(Guid id)
    {
        var getCarConfigurationReply = await _carStorageServiceClient.GetCarConfigurationAsync(new()
        {
            CarConfigurationId = id.ToString()
        });

        if (getCarConfigurationReply.Result != GetCarConfigurationReply.Types.GetCarConfigurationResult.Success)
        {
            return NotFound();
        }

        var carConfiguration = getCarConfigurationReply.CarConfiguration;

        var getCarReply = await _carStorageServiceClient.GetCarAsync(new()
        {
            CarId = carConfiguration.CarId
        });

        if (getCarReply.Result != GetCarReply.Types.GetCarResult.Success)
        {
            return NotFound();
        }

        var car = getCarReply.Car;

        return View("~/Views/Configuration/id/Index.cshtml", new ConfigurationViewModel
        {
            Car = car,
            Configuration = carConfiguration
        });
    }
}