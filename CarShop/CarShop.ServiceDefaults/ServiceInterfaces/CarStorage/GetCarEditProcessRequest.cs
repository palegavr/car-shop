namespace CarShop.ServiceDefaults.ServiceInterfaces.CarStorage;

public class GetCarEditProcessRequest
{
    public long AdminId { get; set; }
    public long CarId { get; set; }
}