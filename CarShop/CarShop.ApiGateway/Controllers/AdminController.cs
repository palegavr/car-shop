using System.Net.Mime;
using System.Security.Claims;
using CarShop.ServiceDefaults.ServiceInterfaces.AdminService;
using CarShop.ServiceDefaults.ServiceInterfaces.ApiGateway;
using CarShop.ServiceDefaults.ServiceInterfaces.CarStorage;
using CarShop.ServiceDefaults.ServiceInterfaces.FileService;
using CarShop.ServiceDefaults.Utils;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace CarShop.ApiGateway.Controllers;

[Authorize]
[Route("api/[controller]")]
public class AdminController
    (FileServiceClient _fileServiceClient,
        CarStorageClient _carStorageClient): ControllerBase
{
    [HttpPost]
    [Route("uploadimage")]
    [Consumes(MediaTypeNames.Multipart.FormData)]
    public async Task<IActionResult> UploadImageAsync([FromForm(Name = "images")] IFormFileCollection formFiles)
    {
        List<string> publicPathes = new(formFiles.Count);
        foreach (IFormFile formFile in formFiles)
        {
            byte[] buffer = new byte [formFile.Length];
            if ((await formFile.OpenReadStream().ReadAsync(buffer, 0, buffer.Length)) == buffer.Length)
            {
                var publicPath = await _fileServiceClient.SaveCatalogImageAsync(new SaveCatalogImageRequest
                {
                    ImageBytes = buffer,
                    FileExtention = Path.GetExtension(formFile.FileName)
                });

                if (publicPath is not null)
                {
                    publicPathes.Add(publicPath);
                }
            }
        }
        
        return Ok(publicPathes);
    }

    [HttpPost]
    [Route("editcar/{id:long}/process")]
    [Consumes(MediaTypeNames.Application.Json)]
    public async Task<IActionResult> EditCarProcessAsync(
        [FromRoute] long id,
        [FromBody] CarEditProcessData carEditProcessData)
    {
        var additionalCarOptions = carEditProcessData.AdditionalCarOptions;
        if (!ModelState.IsValid ||
            (carEditProcessData.FuelType != FuelType.Electric && carEditProcessData.EngineCapacity == 0) ||
            (additionalCarOptions.Length > 0 && additionalCarOptions[0].Id < 0) ||
            additionalCarOptions // Есть элементы с одинаковым типом
                .GroupBy(option => option.Type)
                .Where(group => group.Count() > 1)
                .Select(group => group.Key).Any())
        {
            return BadRequest();
        }

        long? adminId = Utils.GetAdminIdFromClaimsPrincipal(User);
        if (adminId is null)
        {
            return Problem();
        }

        if ((await _carStorageClient.GetCarAsync(id)) is null)
        {
            return NotFound();
        }

        var carEditProcess = new CarEditProcess
        {
            AdminId = adminId.Value,
            CarId = id,
            Process = carEditProcessData,
        };

        await _carStorageClient.UpdateOrCreateCarEditProcessAsync(carEditProcess);
        
        return Ok();
    }

    [HttpPost]
    [Route("editcar/{id:long}/applychanges")]
    [Consumes(MediaTypeNames.Application.Json)]
    public async Task<IActionResult> EditCarApplyChangesAsync([FromRoute] long id)
    {
        long? adminId = Utils.GetAdminIdFromClaimsPrincipal(User);
        if (adminId is null)
        {
            return Problem();
        }

        CarEditProcess? carEditProcess = await _carStorageClient.GetCarEditProcessAsync(new()
        {
            AdminId = adminId.Value,
            CarId = id
        });

        if (carEditProcess is null)
        {
            return NotFound();
        }

        if (!await _carStorageClient.UpdateCarAsync(id, new UpdateCarRequest
            {
                Brand = carEditProcess.Process.Brand,
                Model = carEditProcess.Process.Model,
                Color = carEditProcess.Process.Color,
                Count = carEditProcess.Process.Count,
                FuelType = carEditProcess.Process.FuelType,
                EngineCapacity = carEditProcess.Process.EngineCapacity,
                CorpusType = carEditProcess.Process.CorpusType,
                PriceForStandartConfiguration = carEditProcess.Process.Price,
                ImageUrl = carEditProcess.Process.Image,
                BigImageURLs = carEditProcess.Process.BigImages,
                AdditionalCarOptionsJson = carEditProcess.Process.AdditionalCarOptionsJson,
            }))
        {
            return Problem();
        }

        if (!await _carStorageClient.DeleteCarEditProcessAsync(new DeleteEditProcessRequest
            {
                AdminId = adminId.Value,
                CarId = id
            }))
        {
            return Problem();
        }

        var carConfigurations = await _carStorageClient.GetCarConfigurationAsync(id);
        if (carConfigurations is null)
        {
            return Problem();
        }
        
        foreach (var carConfiguration in carConfigurations)
        {
            bool[] conditions = new bool[4];
            
            var airConditionerOption = carEditProcess.Process.AdditionalCarOptions
                .SingleOrDefault(o => o.Type == AdditionalCarOptionType.AirConditioner);
            var heatedDriversSeatOption = carEditProcess.Process.AdditionalCarOptions
                .SingleOrDefault(o => o.Type == AdditionalCarOptionType.HeatedDriversSeat);
            var seatHeightAdjustmentOption = carEditProcess.Process.AdditionalCarOptions
                .SingleOrDefault(o => o.Type == AdditionalCarOptionType.SeatHeightAdjustment);
            var differentCarColorOption = carEditProcess.Process.AdditionalCarOptions
                .SingleOrDefault(o => o.Type == AdditionalCarOptionType.DifferentCarColor);
            
            conditions[0] = (carConfiguration.AirConditioner && airConditionerOption == null) ||
                            (!carConfiguration.AirConditioner && airConditionerOption?.IsRequired == true);
            conditions[1] = (carConfiguration.HeatedDriversSeat && heatedDriversSeatOption == null) ||
                            (!carConfiguration.HeatedDriversSeat && heatedDriversSeatOption?.IsRequired == true);
            conditions[2] = (carConfiguration.SeatHeightAdjustment && seatHeightAdjustmentOption == null) ||
                            (!carConfiguration.SeatHeightAdjustment && seatHeightAdjustmentOption?.IsRequired == true);
            conditions[3] = (carConfiguration.DifferentCarColor is not null && differentCarColorOption == null) ||
                            (carConfiguration.DifferentCarColor is null && differentCarColorOption?.IsRequired == true);
            
            if (conditions.Contains(true))
            {
                carConfiguration.IsAvaliable = false;
                await _carStorageClient.UpdateCarConfigurationAsync(carConfiguration);
            }
            
        }
        
        return Ok();
    }

    [HttpDelete]
    [Route("car/{id:long}")]
    [Authorize(Roles = Role.Admin.Car.Delete)]
    public async Task<IActionResult> DeleteCarAsync([FromRoute] long id)
    {
        if (await _carStorageClient.GetCarAsync(id) is null)
        {
            return NotFound();
        }
        
        bool deleted = await _carStorageClient.DeleteCarAsync(id);
        return deleted ? Ok() : Problem();
    }
}