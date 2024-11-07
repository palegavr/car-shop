using CarShop.CarStorage.Database.Entities.Car;

namespace CarShop.CarStorage.Extensions;

public static class FuelTypeExtensions
{
    public static CarStorageService.Grpc.Car.Types.FuelType ToGrpcMessage(this FuelType fuelType)
    {
        return (CarStorageService.Grpc.Car.Types.FuelType)fuelType;
    }
    
    public static FuelType FromGrpcMessage(this CarStorageService.Grpc.Car.Types.FuelType fuelType)
    {
        return (FuelType)fuelType;
    }
}