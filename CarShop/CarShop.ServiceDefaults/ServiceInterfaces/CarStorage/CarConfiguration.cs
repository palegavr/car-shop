using System.ComponentModel.DataAnnotations;

namespace CarShop.ServiceDefaults.ServiceInterfaces.CarStorage;

public class CarConfiguration
{
    [Key]
    public Guid Id { get; set; }
    public long CarId { get; set; }
    public bool AirConditioner { get; set; } = false;
    public bool HeatedDriversSeat { get; set; } = false;
    public bool SeatHeightAdjustment { get; set; } = false;
    public string? DifferentCarColor { get; set; } = null;
    public bool IsAvaliable { get; set; } = true;
}