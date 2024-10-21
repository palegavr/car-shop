using System.Net.Mime;
using CarShop.ServiceDefaults.ServiceInterfaces.ApiGateway;
using CarShop.ServiceDefaults.ServiceInterfaces.CarStorage;
using CarShop.ServiceDefaults.ServiceInterfaces.FileService;
using Google.Protobuf.Compiler;
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

        long adminId = long.Parse(User.Claims.FirstOrDefault(claim => claim.Type == "id")?.Value ?? "-1");
        if (adminId <= 0)
        {
            return Problem();
        }

        var carEditProcess = new CarEditProcess
        {
            AdminId = adminId,
            CarId = id,
            Process = carEditProcessData,
        };

        carEditProcess = await _carStorageClient.UpdateOrCreateCarEditProcessAsync(carEditProcess);
        
        return Ok();
    }
}