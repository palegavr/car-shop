using System.Collections;
using System.ComponentModel.DataAnnotations;
using System.Text.Json;
using System.Text.Json.Serialization;
using CarShop.CarStorageService.Grpc;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;

namespace CarShop.Web.Models;

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
    public double Price { get; set; }

    [ModelBinder(Name = "color")]
    [Required]
    public string Color { get; set; }

    [ModelBinder(Name = "engine_capacity")]
    [Required]
    public double EngineCapacity { get; set; }

    [ModelBinder(Name = "corpus_type")]
    [Required]
    public Car.Types.CorpusType CorpusType { get; set; }

    [ModelBinder(Name = "fuel_type")]
    [Required]
    public Car.Types.FuelType FuelType { get; set; }

    [ModelBinder(Name = "count")]
    [Required]
    public int Count { get; set; }

    [ModelBinder(Name = "image")] public IFormFile? Image { get; set; }

    [ModelBinder(Name = "big_images")] public IFormFileCollection? BigImages { get; set; }

    [ModelBinder(Name = "additional_car_options")]
    public string? AdditionalCarOptionsJson { get; set; }

    [BindNever]
    [JsonIgnore]
    public List<AdditionalCarOptionPayload> AdditionalCarOptions
    {
        get
        {
            if (AdditionalCarOptionsJson is null)
            {
                return [];
            }

            var additionalCarOptions = JsonSerializer.Deserialize<List<AdditionalCarOptionPayload>>(AdditionalCarOptionsJson);
            if (additionalCarOptions is null)
            {
                throw new JsonException("При десериализации AdditionalCarOptions был получен null.");
            }

            return additionalCarOptions;
        }
    }
}

public class AdditionalCarOptionPayload
{
    [JsonPropertyName("type")]
    [JsonRequired]
    public AdditionalCarOption.Types.Type Type { get; set; }
    
    [JsonPropertyName("price")]
    [JsonRequired]
    public double Price { get; set; }
    
    [JsonPropertyName("isRequired")]
    [JsonRequired]
    public bool IsRequired { get; set; }
}

