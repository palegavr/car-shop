using CarShop.CarStorage.Database.Entities;
using CarShop.CarStorage.Database.Entities.Car;
using Google.Protobuf.Collections;

namespace CarShop.CarStorage.Extensions;

public static class CarExtensions
{
    public static CarStorageService.Grpc.Car ToGrpcMessage(this Car car)
    {
        return new()
        {
            Id = car.Id,
            Brand = car.Brand,
            CorpusType = car.CorpusType.ToGrpcMessage(),
            Color = car.Color,
            Count = car.Count,
            Model = car.Model,
            FuelType = car.FuelType.ToGrpcMessage(),
            EngineCapacity = car.EngineCapacity,
            ImageUrl = car.ImageUrl,
            PriceForStandartConfiguration = car.PriceForStandartConfiguration,
            BigImageUrls = { car.BigImageURLs },
            AdditionalCarOptions = { from option in car.AdditionalCarOptions select option.ToGrpcMessage() },
        };
    }

    public static Car FromGrpcMessage(this CarStorageService.Grpc.Car carMessage)
    {
        return new()
        {
            Id = carMessage.Id,
            Brand = carMessage.Brand,
            CorpusType = carMessage.CorpusType.FromGrpcMessage(),
            FuelType = carMessage.FuelType.FromGrpcMessage(),
            Color = carMessage.Color,
            Count = carMessage.Count,
            Model = carMessage.Model,
            EngineCapacity = carMessage.EngineCapacity,
            ImageUrl = carMessage.ImageUrl,
            BigImageURLs = carMessage.BigImageUrls.ToArray(),
            PriceForStandartConfiguration = carMessage.PriceForStandartConfiguration,
            AdditionalCarOptions =
                (from option in carMessage.AdditionalCarOptions
                    select option.FromGrpcMessage()).ToList(),
        };
    }
}