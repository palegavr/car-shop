using System.Net.Mime;
using System.Security.Claims;
using CarShop.AdminService.Grpc;
using CarShop.ApiGateway.Models;
using CarShop.CarStorageService.Grpc;
using CarShop.FileService.Grpc;
using CarShop.ServiceDefaults.ServiceInterfaces.AdminService;
using CarShop.ServiceDefaults.Services;
using CarShop.ServiceDefaults.Utils;
using Google.Protobuf;
using Google.Protobuf.Collections;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace CarShop.ApiGateway.Controllers.Admin;

[Authorize]
[Route("api/[controller]")]
public class AdminController(
    FileService.Grpc.FileService.FileServiceClient _fileServiceClient,
    CarStorageService.Grpc.CarStorageService.CarStorageServiceClient _carStorageClient,
    AdminService.Grpc.AdminService.AdminServiceClient _adminServiceClient,
    PasswordGenerator _passwordGenerator) : ControllerBase
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

    [HttpPost]
    [Route("car")]
    [Authorize(Roles = Role.Admin.Car.Add)]
    public async Task<IActionResult> AddCarAsync(
        [FromRoute] long id,
        [FromBody] CarEditProcessDataPayload carData)
    {
        if (!ModelState.IsValid)
        {
            return BadRequest();
        }

        var addCarReply = await _carStorageClient.AddCarAsync(new()
        {
            Car = new()
            {
                Brand = carData.Brand,
                Model = carData.Model,
                Count = carData.Count,
                FuelType = carData.FuelType,
                ImageUrl = carData.ImageUrl,
                BigImageUrls = { carData.BigImageUrls },
                CorpusType = carData.CorpusType,
                PriceForStandartConfiguration = carData.Price,
                Color = carData.Color,
                EngineCapacity = carData.EngineCapacity,
                AdditionalCarOptions = { carData.AdditionalCarOptions },
            }
        });

        return addCarReply.Result switch
        {
            AddCarReply.Types.AddCarResult.BadRequest => BadRequest(),
            AddCarReply.Types.AddCarResult.Success => Ok(new { id = addCarReply.Car.Id }),
            _ => Problem(),
        };
    }

    [HttpGet]
    [Route("admins")]
    [Authorize]
    public async Task<IActionResult> GetAdminsAsync(
        [FromRoute] long id,
        [FromBody] GetAdminsPayload? getAdminsPayload)
    {
        if (!ModelState.IsValid)
        {
            return BadRequest();
        }

        var getAccountsRequest = new GetAccountsRequest()
        {
            SortType = getAdminsPayload?.SortType ?? GetAccountsRequest.Types.SortType.Asc,
        };
        if (getAdminsPayload?.SortBy is not null)
        {
            getAccountsRequest.SortBy = getAccountsRequest.SortBy;
        }

        if (getAdminsPayload?.MinPriority is not null)
        {
            getAccountsRequest.MinPriority = getAdminsPayload.MinPriority.Value;
        }
        
        if (getAdminsPayload?.MaxPriority is not null)
        {
            getAccountsRequest.MaxPriority = getAdminsPayload.MaxPriority.Value;
        }

        if (getAdminsPayload?.HaveRoles is not null)
        {
            getAccountsRequest.HaveRoles.AddRange(getAdminsPayload.HaveRoles);
        }

        if (getAdminsPayload?.Banned is not null)
        {
            getAccountsRequest.Banned = getAdminsPayload.Banned.Value;
        }
        var getAccountsReply = await _adminServiceClient.GetAccountsAsync(getAccountsRequest);
        var adminAccounts = getAccountsReply.Accounts.ToArray();
        foreach (var adminAccount in adminAccounts)
        {
            adminAccount.Password = "";
        }
        return Ok(adminAccounts);
    }

    [Authorize]
    [HttpPost]
    [Route("account/{id:long?}")]
    public async Task<IActionResult> AccountActionAsync(
        [FromRoute] long? id,
        [FromBody] AccountActionPayload payload)
    {
        long adminId = Utils.GetAdminIdFromClaimsPrincipal(User) ?? throw new ArgumentNullException();
        int performingAdminPriority = Utils.GetPriorityFromClaimsPrincipal(User) ?? throw new ArgumentNullException();
        id ??= 0;

        if (!ModelState.IsValid ||
            (id <= 0 && payload.ActionType != AccountAction.Create))
        {
            return BadRequest(ModelState.Values.Select(entry => entry.Errors));
        }

        var getAccountReply = payload.ActionType != AccountAction.Create
            ? await _adminServiceClient.GetAccountAsync(new()
            {
                AccountId = id.Value
            })
            : null;

        if (getAccountReply?.Result == GetAccountReply.Types.GetAccountResult.AccountNotFound)
        {
            return NotFound();
        }

        if (!payload.ActionIsAllowed(
                roles: User.Claims
                    .Where(claim => claim.Type == ClaimTypes.Role)
                    .Select(claim => claim.Value)
                    .ToArray(),
                performingAdminId: adminId,
                adminId: id.Value,
                performingAdminPriority: performingAdminPriority,
                adminPriority: getAccountReply?.Account.Priority ?? default
            ))
        {
            return Forbid();
        }

        var accountActionHandler = new AccountActionHandler(id.Value, _passwordGenerator, _adminServiceClient);

        return payload.ActionType switch
        {
            AccountAction.Create => await accountActionHandler.Create(payload.Data!.Email!),
            AccountAction.ChangePassword => await accountActionHandler.ChangePassword(
                payload.Data!.Password!,
                payload.Data!.OldPassword!,
                performingAdminId: adminId),
            AccountAction.Ban => await accountActionHandler.Ban(),
            AccountAction.Unban => await accountActionHandler.Unban(),
            AccountAction.GiveRole => await accountActionHandler.GiveRoles([payload.Data!.Role!]),
            AccountAction.TakeRole => await accountActionHandler.TakeRoles([payload.Data!.Role!]),
            AccountAction.SetPriority => await accountActionHandler.SetPriority(payload.Data!.Priority!.Value),
            _ => throw new ArgumentOutOfRangeException()
        };
    }
}