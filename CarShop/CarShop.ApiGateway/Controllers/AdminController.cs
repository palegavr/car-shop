using System.Net.Mime;
using System.Security.Claims;
using CarShop.ApiGateway.Models;
using CarShop.CarStorageService.Grpc;
using CarShop.FileService.Grpc;
using CarShop.ServiceDefaults.ServiceInterfaces.AdminService;
using CarShop.ServiceDefaults.Utils;
using Google.Protobuf;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace CarShop.ApiGateway.Controllers;

[Authorize]
[Route("api/[controller]")]
public class AdminController(
    FileService.Grpc.FileService.FileServiceClient _fileServiceClient,
    CarStorageService.Grpc.CarStorageService.CarStorageServiceClient _carStorageClient) : ControllerBase
{
    [HttpPost]
    [Route("uploadimage")]
    [Consumes(MediaTypeNames.Multipart.FormData)]
    [DisableRequestSizeLimit]
    public async Task<IActionResult> UploadImageAsync([FromForm(Name = "images")] IFormFileCollection formFiles)
    {
        List<string> publicPathes = new(formFiles.Count);
        foreach (IFormFile formFile in formFiles)
        {
            var result = await _fileServiceClient.SaveCatalogImageAsync(new SaveCatalogImageRequest
            {
                ImageBytes = await ByteString.FromStreamAsync(formFile.OpenReadStream()),
                FileExtention = Path.GetExtension(formFile.FileName)
            });

            if (result.Result == SaveCatalogImageResult.Success)
            {
                publicPathes.Add(result.PublicPath);
            }
        }

        return Ok(publicPathes);
    }

    [HttpPost]
    [Route("editcar/{id:long}/process")]
    [Consumes(MediaTypeNames.Application.Json)]
    public async Task<IActionResult> EditCarProcessAsync(
        [FromRoute] long id,
        [FromBody] CarEditProcessDataPayload carEditProcessDataPayload)
    {
        if (!ModelState.IsValid)
        {
            return BadRequest();
        }

        long? adminId = Utils.GetAdminIdFromClaimsPrincipal(User);
        if (adminId is null)
        {
            return Problem();
        }

        var carEditProcess = new CarEditProcess
        {
            AdminId = adminId.Value,
            CarId = id,
            Data = new CarEditProcessData
            {
                Brand = carEditProcessDataPayload.Brand,
                Model = carEditProcessDataPayload.Model,
                Count = carEditProcessDataPayload.Count,
                FuelType = carEditProcessDataPayload.FuelType,
                ImageUrl = carEditProcessDataPayload.ImageUrl,
                BigImageUrls = { carEditProcessDataPayload.BigImageUrls },
                CorpusType = carEditProcessDataPayload.CorpusType,
                Color = carEditProcessDataPayload.Color,
                EngineCapacity = carEditProcessDataPayload.EngineCapacity,
                Price = carEditProcessDataPayload.Price,
                AdditionalCarOptions = { carEditProcessDataPayload.AdditionalCarOptions },
            },
        };

        var updateOrCreateCarEditProcessReply = await _carStorageClient.UpdateOrCreateCarEditProcessAsync(new()
        {
            CarEditProcess = carEditProcess,
        });

        if (updateOrCreateCarEditProcessReply.Result ==
            UpdateOrCreateCarEditProcessReply.Types.UpdateOrCreateCarEditProcessResult.CarNotFound)
        {
            return NotFound();
        }

        if (updateOrCreateCarEditProcessReply.Result ==
            UpdateOrCreateCarEditProcessReply.Types.UpdateOrCreateCarEditProcessResult.BadRequest)
        {
            return BadRequest();
        }

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

        var getCarEditProcessReply =
            await _carStorageClient.GetCarEditProcessAsync(new()
            {
                AdminId = adminId.Value,
                CarId = id
            });

        if (getCarEditProcessReply.Result == GetCarEditProcessReply.Types.GetCarEditProcessResult.NotFound)
        {
            return NotFound();
        }

        var carEditProcess = getCarEditProcessReply.CarEditProcess;

        var updateCarReply = await _carStorageClient.UpdateCarAsync(new()
        {
            CarId = carEditProcess.CarId,
            CorpusType = carEditProcess.Data.CorpusType,
            PriceForStandartConfiguration = carEditProcess.Data.Price,
            Color = carEditProcess.Data.Color,
            EngineCapacity = carEditProcess.Data.EngineCapacity,
            Brand = carEditProcess.Data.Brand,
            Model = carEditProcess.Data.Model,
            Count = carEditProcess.Data.Count,
            FuelType = carEditProcess.Data.FuelType,
            ImageUrl = carEditProcess.Data.ImageUrl,
            BigImageUrls = { carEditProcess.Data.BigImageUrls },
            AdditionalCarOptions = { carEditProcess.Data.AdditionalCarOptions },
            UpdateBigImageUrls = true,
            UpdateAdditionalCarOptions = true
        });

        if (updateCarReply.Result != UpdateCarReply.Types.UpdateCarResult.Success)
        {
            return Problem();
        }

        var deleteCarEditProcessReply = await _carStorageClient.DeleteCarEditProcessAsync(new()
        {
            AdminId = adminId.Value,
            CarId = id
        });

        if (deleteCarEditProcessReply.Result != DeleteCarEditProcessReply.Types.DeleteCarEditProcessResult.Success)
        {
            return Problem();
        }

        return Ok();
    }

    [HttpDelete]
    [Route("car/{id:long}")]
    [Authorize(Roles = Role.Admin.Car.Delete)]
    public async Task<IActionResult> DeleteCarAsync([FromRoute] long id)
    {
        var deleteCarReply = await _carStorageClient.DeleteCarAsync(new()
        {
            CarId = id
        });
        
        if (deleteCarReply.Result == DeleteCarReply.Types.DeleteCarResult.CarNotFound)
        {
            return NotFound();
        }
        
        return Ok();
    }
}