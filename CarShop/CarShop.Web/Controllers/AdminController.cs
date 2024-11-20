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

		[Authorize]
		[HttpGet]
		public async Task<IActionResult> IndexAsync([FromQuery] string? email)
		{
			var getAccountsReply = await _adminServiceClient.GetAccountsAsync(new());
			string html = await System.IO.File.ReadAllTextAsync("wwwroot/admin.html");

			return View(new AdminViewModel
			{
				BodyHtmlContent = ExtractTagContent(html, "body"),
				HeadHtmlContent = ExtractTagContent(html, "head"),
				PerformingAdmin = new PerformingAdmin()
				{
					Id = Utils.GetAdminIdFromClaimsPrincipal(User) ?? throw new Exception("Can not extract id from JWT token"),
					Email = Utils.GetEmailFromClaimsPrincipal(User) ?? throw new Exception("Can not extract email from JWT token"),
					Priority = Utils.GetPriorityFromClaimsPrincipal(User) ?? throw new Exception("Can not extract priority from JWT token"),
					Roles = User.Claims
						.Where(claim => claim.Type == ClaimTypes.Role)
						.Select(claim => claim.Value)
						.ToList(),
				},
				AdminAccounts = getAccountsReply.Accounts.ToArray(),
				AdminEmail = email
			});
		}

		[HttpGet]
		[Route("login")]
		public async Task<IActionResult> LoginAsync()
		{
			if (Utils.GetAdminIdFromClaimsPrincipal(User) is not null)
			{
				return RedirectToAction("Index");
			}
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
