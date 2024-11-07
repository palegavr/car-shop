using CarShop.CarStorage.Database.Entities.Car;
using CarShop.CarStorage.Database.Entities.CarEditProcess;

namespace CarShop.CarStorage.Extensions;

public static class CarEditProcessExtensions
{
    public static CarShop.CarStorageService.Grpc.CarEditProcess ToGrpcMessage(this CarEditProcess carEditProcess)
    {
        return new()
        {
            AdminId = carEditProcess.AdminId,
            CarId = carEditProcess.CarId,
            Data = carEditProcess.Process.ToGrpcMessage()
        };
    }

    public static CarEditProcess FromGrpcMessage(this CarShop.CarStorageService.Grpc.CarEditProcess carEditProcess)
    {
        return new()
        {
            AdminId = carEditProcess.AdminId,
            CarId = carEditProcess.CarId,
            Process = carEditProcess.Data.FromGrpcMessage(),
        };
    }
}