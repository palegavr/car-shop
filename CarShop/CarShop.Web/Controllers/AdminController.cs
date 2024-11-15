using Microsoft.AspNetCore.Connections.Features;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Net;
using System.Net.Mime;
using System.Security.Claims;
using System.Text.Json;
using System.Text.RegularExpressions;
using CarShop.AdminService.Grpc;
using CarShop.CarStorageService.Grpc;
using CarShop.FileService.Grpc;
using CarShop.ServiceDefaults;
using CarShop.ServiceDefaults.Utils;
using CarShop.Web.Models;
using CarShop.Web.Models.Admin;
using Google.Protobuf;
using Microsoft.AspNetCore.Authorization;
using LoginRequest = CarShop.AdminService.Grpc.LoginRequest;

namespace CarShop.Web.Controllers
{
	[Route("[controller]")]
	public class AdminController(
		CarStorageService.Grpc.CarStorageService.CarStorageServiceClient _carStorageClient, 
		AdminService.Grpc.AdminService.AdminServiceClient _adminServiceClient,
		FileService.Grpc.FileService.FileServiceClient _fileServiceClient) : Controller
	{
		public static readonly string[] ALLOWED_IMAGES_EXTENTIONS = ["jpg", "jpeg", "png"];

		public IActionResult Index()
		{
			return RedirectToAction("AddCar");
		}

		[Authorize]
		[HttpGet]
		[Route("addcar")]
		public async Task<IActionResult> AddCarAsync()
		{
			return View();
		}

		[Authorize]
		[HttpPost]
		[Route("addcar")]
		[Consumes(MediaTypeNames.Multipart.FormData)]
		public async Task<IActionResult> AddCarAsync([FromForm] AddCarFormModel addCarFormModel)
		{
			if (!ModelState.IsValid)
			{
				return BadRequest();
			}
			
			// Загружаем картинку для /catalog, если была отправлена пользователем
			string imageUrl = string.Empty;
			if (addCarFormModel.Image is not null && addCarFormModel.Image.Length > 0)
			{
				var saveCatalogImageReply = await _fileServiceClient.SaveCatalogImageAsync(new()
				{
					ImageBytes = await ByteString.FromStreamAsync(addCarFormModel.Image.OpenReadStream()),
					FileExtention = Path.GetExtension(addCarFormModel.Image.FileName)
				});

				if (saveCatalogImageReply.Result == SaveCatalogImageResult.Success)
				{
					imageUrl = saveCatalogImageReply.PublicPath;
				}
			}

			// Загружаем картинки для /catalog/{id}, если были отправлены пользователем
			List<string> bigImageUrls = new();
			if (addCarFormModel.BigImages is not null)
			{
				foreach (var bigImage in addCarFormModel.BigImages)
				{
					if (bigImage.Length > 0)
					{
						var saveCatalogImageReply = await _fileServiceClient.SaveCatalogImageAsync(new()
						{
							ImageBytes = await ByteString.FromStreamAsync(bigImage.OpenReadStream()),
							FileExtention = Path.GetExtension(bigImage.FileName)
						});

						if (saveCatalogImageReply.Result == SaveCatalogImageResult.Success)
						{
							bigImageUrls.Add(saveCatalogImageReply.PublicPath);
						}
					}
				}
			}

			var addCarReply = await _carStorageClient.AddCarAsync(new AddCarRequest
			{
				Car = new Car
				{
					Brand = addCarFormModel.Brand,
					Model = addCarFormModel.Model,
					PriceForStandartConfiguration = addCarFormModel.Price,
					Color = addCarFormModel.Color,
					EngineCapacity = addCarFormModel.EngineCapacity,
					CorpusType = addCarFormModel.CorpusType,
					FuelType = addCarFormModel.FuelType,
					Count = addCarFormModel.Count,
					ImageUrl = imageUrl,
					BigImageUrls = { bigImageUrls },
					AdditionalCarOptions =
					{
						addCarFormModel.AdditionalCarOptions
							.Select(optionPayload => new AdditionalCarOption
							{
								Type = optionPayload.Type,
								IsRequired = optionPayload.IsRequired,
								Price = optionPayload.Price
							})
					},
				}
			});

			if (addCarReply.Result == AddCarReply.Types.AddCarResult.BadRequest)
			{
				return BadRequest();
			}
			
			var car = addCarReply.Car;

			return Redirect($"/catalog/{car.Id}");
		}

		[HttpGet]
		[Route("login")]
		public async Task<IActionResult> LoginAsync()
		{
			return View(new LoginViewModel());
		}
		
		[HttpPost]
		[Route("login")]
		public async Task<IActionResult> LoginAsync(
			[FromForm(Name = "email")] string login, 
			[FromForm(Name = "password")] string password)
		{
			var response = await _adminServiceClient.LoginAsync(new LoginRequest
			{
				Email = login ?? string.Empty,
				Password = password ?? string.Empty
			});
			
			if (response.Result == LoginReply.Types.LoginResult.Success)
			{
				Response.Cookies.SetAccessTokenCookie(response.AccessToken);
				Response.Cookies.SetRefreshTokenCookie(response.RefreshToken);
				return Redirect("/admin");
			}

			var viewModel = new LoginViewModel
			{
				ErrorMessage = "Не удалось войти в аккаунт.",
				Login = login,
				Password = password
			};
			return View(viewModel);
		}

		[Authorize]
		[HttpGet]
		[Route("editcar/{id:long}")]
		public async Task<IActionResult> EditCarAsync([FromRoute] long id)
		{
			long? adminId = Utils.GetAdminIdFromClaimsPrincipal(User);
			if (adminId is null)
			{
				return Problem();
			}

			var getCarEditProcessReply = await _carStorageClient.GetCarEditProcessAsync(new()
			{
				AdminId = adminId.Value,
				CarId = id
			});

			var carEditProcess =
				getCarEditProcessReply.Result == GetCarEditProcessReply.Types.GetCarEditProcessResult.Success
					? getCarEditProcessReply.CarEditProcess
					: null;

			var getCarReply = await _carStorageClient.GetCarAsync(new()
			{
				CarId = id
			});

			if (getCarReply.Result == GetCarReply.Types.GetCarResult.CarNotFound)
			{
				return NotFound();
			}

			var car = getCarReply.Car;
			
			string html = await System.IO.File.ReadAllTextAsync("wwwroot/admin/editcar/id.html");
			string processDataInDbJsonEncoded = JsonSerializer.Serialize(new
			{
				brand = car.Brand,
				model = car.Model,
				price = car.PriceForStandartConfiguration,
				color = car.Color,
				engineCapacity = car.EngineCapacity,
				corpusType = car.CorpusType,
				fuelType = car.FuelType,
				count = car.Count,
				imageUrl = car.ImageUrl,
				bigImageUrls = car.BigImageUrls.ToArray(),
				additionalCarOptions = JsonSerializer.Serialize(car.AdditionalCarOptions
					.Select(option => new
					{
						type = option.Type,
						price = option.Price,
						isRequired = option.IsRequired
					}).ToArray())
			});

			return View(new EditCarViewModel
			{
				BodyHtmlContent = ExtractTagContent(html, "body"), 
				HeadHtmlContent = ExtractTagContent(html, "head"),
				ProcessDataInDbJsonEncoded = processDataInDbJsonEncoded,
				CurrentProcessDataJsonEncoded = carEditProcess is not null ? 
					JsonSerializer.Serialize(new
					{
						brand = carEditProcess.Data.Brand,
						model = carEditProcess.Data.Model,
						price = carEditProcess.Data.Price,
						color = carEditProcess.Data.Color,
						engineCapacity = carEditProcess.Data.EngineCapacity,
						corpusType = carEditProcess.Data.CorpusType,
						fuelType = carEditProcess.Data.FuelType,
						count = carEditProcess.Data.Count,
						imageUrl = carEditProcess.Data.ImageUrl,
						bigImageUrls = carEditProcess.Data.BigImageUrls.ToArray(),
						additionalCarOptions = JsonSerializer.Serialize(
							carEditProcess.Data.AdditionalCarOptions
							.Select(option => new
							{
								type = option.Type,
								price = option.Price,
								isRequired = option.IsRequired
							}).ToArray())
					})
					: processDataInDbJsonEncoded,
				CarId = id
			});
		}

		[Authorize]
		[HttpGet]
		[Route("account/{id:long}")]
		public async Task<IActionResult> AccountAsync([FromRoute] long id)
		{
			var getAccountReply = await _adminServiceClient.GetAccountAsync(new()
			{
				AccountId = id
			});

			if (getAccountReply.Result == GetAccountReply.Types.GetAccountResult.AccountNotFound)
			{
				return NotFound();
			}
			
			string html = await System.IO.File.ReadAllTextAsync("wwwroot/admin/account/id.html");
			return View(new AccountViewModel
			{
				BodyHtmlContent = ExtractTagContent(html, "body"),
				HeadHtmlContent = ExtractTagContent(html, "head"),
				Administrator = new()
				{
					Id = getAccountReply.Account.Id,
					Priority = getAccountReply.Account.Priority,
					Roles = getAccountReply.Account.Roles.ToList(),
					Banned = getAccountReply.Account.Banned
				},
				PerformingAdministrator = new()
				{
					Id = Utils.GetAdminIdFromClaimsPrincipal(User) ?? throw new Exception(),
					Priority = Utils.GetPriorityFromClaimsPrincipal(User) ?? throw new Exception(),
					Roles = User.Claims
						.Where(claim => claim.Type == ClaimTypes.Role)
						.Select(claim => claim.Value)
						.ToList(),
				}
			});
		}
		
		[NonAction]
		private static string ExtractTagContent(string html, string tagName)
		{
			// Регулярное выражение для поиска содержимого внутри указанного тега
			var regex = new Regex($"<{tagName}.*?>(.*?)</{tagName}>", RegexOptions.Singleline | RegexOptions.IgnoreCase);
			var match = regex.Match(html);
			return match.Success ? match.Groups[1].Value : string.Empty;
		}
	}
}
