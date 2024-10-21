using System.ComponentModel.DataAnnotations;
using System.Text.Json;
using System.Text.Json.Serialization;
using CarShop.ServiceDefaults.ServiceInterfaces.CarStorage;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;

namespace CarShop.ServiceDefaults.ServiceInterfaces.Web;

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
    [Range(0.0, double.MaxValue)]
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
		
    [ModelBinder(Name = "additional_car_options")]
    public string? AdditionalCarOptionsJson { get; set; }
    [BindNever]
    [JsonIgnore]
    public AdditionalCarOption[] AdditionalCarOptions {
        get
        {
            if (AdditionalCarOptionsJson is null)
            {
                return [];
            }

            try
            {
                return JsonSerializer.Deserialize<AdditionalCarOption[]>(AdditionalCarOptionsJson) ?? 
                [new()
                {
                    Id = -1
                }];
            }
            catch (JsonException)
            {
                return [new()
                {
                    Id = -1
                }];
            }
        }
    }
}