namespace CarShop.CarStorage.Database.Entities.Car;

[Flags]
public enum FuelType
{
    Petrol = 1,
    Diesel = 2,
    Gas = 4,
    Electric = 8,
}