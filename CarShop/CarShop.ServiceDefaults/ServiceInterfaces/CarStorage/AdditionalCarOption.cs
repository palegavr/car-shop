using System.ComponentModel.DataAnnotations;

namespace CarShop.ServiceDefaults.ServiceInterfaces.CarStorage;

public class AdditionalCarOption
{
    [Key]
    public long Id { get; set; }
    public AdditionalCarOptionType Type { get; set; }
    public double Price { get; set; } = 0.0;
    public bool IsRequired { get; set; } = false;
}

public enum AdditionalCarOptionType
{
    AirConditioner,
    HeatedDriversSeat,
    SeatHeightAdjustment,
    DifferentCarColor
}