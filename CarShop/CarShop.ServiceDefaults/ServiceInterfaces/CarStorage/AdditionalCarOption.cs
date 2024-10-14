using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace CarShop.ServiceDefaults.ServiceInterfaces.CarStorage;

public class AdditionalCarOption
{
    [Key]
    [JsonIgnore]
    public long Id { get; set; }
    [JsonPropertyName("type")]
    [JsonRequired]
    public AdditionalCarOptionType Type { get; set; }
    [JsonPropertyName("price")]
    [Range(0, double.MaxValue)]
    [JsonRequired]
    public double Price { get; set; } = 0.0;
    [JsonPropertyName("isRequired")]
    [JsonRequired]
    public bool IsRequired { get; set; } = false;
}

public enum AdditionalCarOptionType
{
    AirConditioner,
    HeatedDriversSeat,
    SeatHeightAdjustment,
    DifferentCarColor
}