using CarShop.ServiceDefaults.ServiceInterfaces.CarStorage;
using Microsoft.AspNetCore.Connections.Features;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Net;
using System.Net.Mime;
using System.Text.Json;
using System.Text.RegularExpressions;
using CarShop.ServiceDefaults;
using CarShop.ServiceDefaults.ServiceInterfaces.AdminService;
using CarShop.ServiceDefaults.ServiceInterfaces.ApiGateway;
using CarShop.ServiceDefaults.ServiceInterfaces.Web;
using CarShop.ServiceDefaults.Utils;
using CarShop.Web.Models;
using CarShop.Web.Models.Admin;
using Microsoft.AspNetCore.Authorization;
using static System.Net.Mime.MediaTypeNames;

namespace CarShop.Web.Controllers
{
	[Route("[controller]")]
	public class AdminController(CarStorageClient _carStorageClient, AdminServiceClient _adminServiceClient) : Controller
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
			var additionalCarOptions = addCarFormModel.AdditionalCarOptions;
			if (!ModelState.IsValid ||
			    (addCarFormModel.FuelType != FuelType.Electric && addCarFormModel.EngineCapacity == 0) ||
			    !HaveAllowedImageExtention(addCarFormModel)||
			    (additionalCarOptions.Length > 0 && additionalCarOptions[0].Id < 0) ||
			    additionalCarOptions // Есть элементы с одинаковым типом
				    .GroupBy(option => option.Type)
				    .Where(group => group.Count() > 1)
				    .Select(group => group.Key).Any())
			{
				return BadRequest();
			}

			string imageUrl = string.Empty;
			if (addCarFormModel.Image is not null && addCarFormModel.Image.Length > 0)
			{
				imageUrl = (await SaveImageAsync(addCarFormModel.Image)).Split("wwwroot", 2)[1];
			}

			List<string> bigImageUrls = new();
			if (addCarFormModel.BigImages is not null)
			{
				foreach (var bigImage in addCarFormModel.BigImages)
				{
					if (bigImage.Length > 0)
					{
						bigImageUrls.Add((await SaveImageAsync(bigImage)).Split("wwwroot", 2)[1]);
					}
				}
			}
            

            Car carInDatabase = await _carStorageClient.AddCarAsync(new Car
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
				BigImageURLs = bigImageUrls.ToArray(),
				AdditionalCarOptions = additionalCarOptions.ToList()
			});

			return Redirect($"/catalog/{carInDatabase.Id}");
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
			var responce = await _adminServiceClient.LoginAsync(login, password);
			if (responce.StatusCode == HttpStatusCode.OK)
			{
				TokensPairResponce tokensPairResponce 
					= (await responce.Content.ReadFromJsonAsync<TokensPairResponce>())!;
				
				Response.Cookies.SetAccessTokenCookie(tokensPairResponce.AccessToken);
				Response.Cookies.SetRefreshTokenCookie(tokensPairResponce.RefreshToken);
				return Redirect("/admin");
			}
			else
			{
				var viewModel = new LoginViewModel
				{
					ErrorMessage = "Не удалось войти в аккаунт.",
					Login = login,
					Password = password
				};
				return View(viewModel);
			}
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

			var carEditProcess = await _carStorageClient.GetCarEditProcessAsync(new GetCarEditProcessRequest
			{
				AdminId = adminId.Value,
				CarId = id
			});
			
			var car = await _carStorageClient.GetCarAsync(id);
			if (car is null)
			{
				return NotFound();
			}

			string html = await System.IO.File.ReadAllTextAsync("wwwroot/admin/editcar/id.html");

			string processDataInDbJsonEncoded = JsonSerializer.Serialize(new CarEditProcessData
			{
				Brand = car.Brand,
				Model = car.Model,
				Color = car.Color,
				EngineCapacity = car.EngineCapacity,
				CorpusType = car.CorpusType,
				FuelType = car.FuelType,
				Count = car.Count,
				AdditionalCarOptionsJson = JsonSerializer.Serialize(car.AdditionalCarOptions),
				Image = car.ImageUrl,
				BigImages = car.BigImageURLs,
				Price = car.PriceForStandartConfiguration,
			});
			return View(new EditCarViewModel
			{
				BodyHtmlContent = ExtractTagContent(html, "body"), 
				HeadHtmlContent = ExtractTagContent(html, "head"),
				ProcessDataInDbJsonEncoded = processDataInDbJsonEncoded,
				CurrentProcessDataJsonEncoded = carEditProcess is not null ? 
					JsonSerializer.Serialize(carEditProcess.Process) : processDataInDbJsonEncoded,
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

		[NonAction]
		private bool HaveAllowedImageExtention(AddCarFormModel addCarFormModel)
		{
			if (addCarFormModel.Image is not null &&
				!HaveAllowedImageExtention(addCarFormModel.Image))
				return false;

			if (addCarFormModel.BigImages is not null)
			{
                foreach (var bigImage in addCarFormModel.BigImages)
                {
					if (!HaveAllowedImageExtention(bigImage))
						return false;
                }
            }

			return true;
		}

		[NonAction]
		private bool HaveAllowedImageExtention(IFormFile formFile)
		{
			var extention = Path.GetExtension(formFile.FileName);
			return extention != "" && ALLOWED_IMAGES_EXTENTIONS.Contains(extention.Substring(1).ToLower());
		}

		[NonAction]
		private async Task<string> SaveImageAsync(IFormFile fileForSaving)
		{
			var path = Path.Combine(Directory.GetCurrentDirectory(),
					$"wwwroot/images/route/catalog");
			
			if (!Directory.Exists(path))
			{
				Directory.CreateDirectory(path);
			}

			Guid imageGuid = Guid.NewGuid();
			while(Directory.GetFiles(path).Contains(imageGuid.ToString()))
			{
				imageGuid = Guid.NewGuid();
			}

			string imagePath = Path.Combine(path, imageGuid.ToString() + Path.GetExtension(fileForSaving.FileName));
			using (var stream = new FileStream(imagePath, FileMode.Create))
			{
				await fileForSaving.CopyToAsync(stream);
			}

			return imagePath;
		}
	}
}
