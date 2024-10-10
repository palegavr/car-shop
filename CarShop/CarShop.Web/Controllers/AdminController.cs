﻿using CarShop.ServiceDefaults.CommonTypes;
using CarShop.ServiceDefaults.ServiceInterfaces.CarStorage;
using Microsoft.AspNetCore.Connections.Features;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using System;
using System.ComponentModel.DataAnnotations;
using static System.Net.Mime.MediaTypeNames;

namespace CarShop.Web.Controllers
{
	[Route("[controller]")]
	public class AdminController(CarStorageClient _carStorageClient) : Controller
	{
		public static readonly string[] ALLOWED_IMAGES_EXTENTIONS = ["jpg", "jpeg", "png"];

		[HttpPost]
		[Route("addcar")]
		public async Task<IActionResult> AddCarAsync([FromForm] AddCarFormModel addCarFormModel)
		{
			if (!(Request.ContentType?.StartsWith("multipart/form-data", StringComparison.InvariantCultureIgnoreCase) ?? false) ||
				!ModelState.IsValid ||
				!HaveAllowedImageExtention(addCarFormModel))
			{
				return BadRequest();
			};

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
			});

			return Redirect($"/catalog/{carInDatabase.Id}");
		}

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

		private bool HaveAllowedImageExtention(IFormFile formFile)
		{
			var extention = Path.GetExtension(formFile.FileName);
			return extention != "" && ALLOWED_IMAGES_EXTENTIONS.Contains(extention.Substring(1));
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

	public class AddCarFormModel
	{
		[ModelBinder(Name = "brand")]
		[Required]
        public string Brand { get; set; }

		[ModelBinder(Name = "model")]
		[Required]
		public string Model { get; set; }

		[ModelBinder(Name = "price")]
		[Required]
		[Range(0.0, double.MaxValue, MinimumIsExclusive = true)]
		public double Price { get; set; }

		[ModelBinder(Name = "color")]
		[Required]
		public string Color { get; set; }

		[ModelBinder(Name = "engine_capacity")]
		[Required]
		[Range(0.0, double.MaxValue, MinimumIsExclusive = true)]
		public double EngineCapacity { get; set; }

		[ModelBinder(Name = "corpus_type")]
		[Required]
		public CorpusType CorpusType { get; set; }

		[ModelBinder(Name = "fuel_type")]
		[Required]
		public FuelType FuelType { get; set; }

		[ModelBinder(Name = "count")]
		[Required]
		[Range(0, int.MaxValue)]
		public int Count { get; set; }

		[ModelBinder(Name = "image")]
		public IFormFile? Image { get; set; }

		[ModelBinder(Name = "big_images")]
		public IFormFileCollection? BigImages { get; set; }
	}
}
