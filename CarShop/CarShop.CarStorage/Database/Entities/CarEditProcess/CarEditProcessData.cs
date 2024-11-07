using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json;
using System.Text.Json.Serialization;
using CarShop.CarStorage.Database.Entities.Car;
using Microsoft.AspNetCore.Mvc.ModelBinding;

namespace CarShop.CarStorage.Database.Entities.CarEditProcess;

public class CarEditProcessData
{
    [JsonPropertyName("brand")]
    [Required]
    public string Brand { get; set; }

    [JsonPropertyName("model")]
    [Required]
    public string Model { get; set; }

    [JsonPropertyName("price")]
    [Required]
    [Range(0.0, double.MaxValue, MinimumIsExclusive = true)]
    public double Price { get; set; }

    [JsonPropertyName("color")]
    [Required]
    public string Color { get; set; }

    [JsonPropertyName("engine_capacity")]
    [Required]
    [Range(0.0, double.MaxValue)]
    public double EngineCapacity { get; set; }

    [JsonPropertyName("corpus_type")]
    [Required]
    public CorpusType CorpusType { get; set; }

    [JsonPropertyName("fuel_type")]
    [Required]
    public FuelType FuelType { get; set; }

    [JsonPropertyName("count")]
    [Required]
    [Range(0, int.MaxValue)]
    public int Count { get; set; }

    [JsonPropertyName("image_url")]
    public string Image { get; set; }

    [JsonPropertyName("big_image_urls")]
    public string[] BigImages { get; set; }
		
    [JsonPropertyName("additional_car_options")]
    public string? AdditionalCarOptionsJson { get; set; }
    [BindNever]
    [JsonIgnore]
    [NotMapped]
    public AdditionalCarOption.AdditionalCarOption[] AdditionalCarOptions {
        get
        {
            if (AdditionalCarOptionsJson is null)
            {
                return [];
            }

            try
            {
                return JsonSerializer.Deserialize<AdditionalCarOption.AdditionalCarOption[]>(AdditionalCarOptionsJson) ?? 
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