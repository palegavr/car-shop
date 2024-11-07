using System.Text.Json;
using CarShop.CarStorage.Database.Entities.AdditionalCarOption;
using CarShop.CarStorage.Database.Entities.Car;
using CarShop.CarStorage.Database.Entities.CarEditProcess;

namespace CarShop.CarStorage.Extensions;

public static class CarEditProcessDataExtensions
{
    public static CarShop.CarStorageService.Grpc.CarEditProcessData ToGrpcMessage(
        this CarEditProcessData carEditProcessData)
    {
        return new()
        {
            Brand = carEditProcessData.Brand,
            Model = carEditProcessData.Model,
            EngineCapacity = carEditProcessData.EngineCapacity,
            CorpusType = carEditProcessData.CorpusType.ToGrpcMessage(),
            FuelType = carEditProcessData.FuelType.ToGrpcMessage(),
            Color = carEditProcessData.Color,
            Count = carEditProcessData.Count,
            Price = carEditProcessData.Price,
            ImageUrl = carEditProcessData.Image,
            BigImageUrls = { carEditProcessData.BigImages },
            AdditionalCarOptions = { carEditProcessData.AdditionalCarOptions.ToGrpcMessage() },
        };
    }

    public static CarEditProcessData FromGrpcMessage(
        this CarShop.CarStorageService.Grpc.CarEditProcessData carEditProcessData)
    {
        AdditionalCarOption[] additionalCarOptions = 
            carEditProcessData.AdditionalCarOptions.FromGrpcMessage().ToArray();
        return new()
        {
            Brand = carEditProcessData.Brand,
            Model = carEditProcessData.Model,
            EngineCapacity = carEditProcessData.EngineCapacity,
            CorpusType = carEditProcessData.CorpusType.FromGrpcMessage(),
            Color = carEditProcessData.Color,
            Count = carEditProcessData.Count,
            Price = carEditProcessData.Price,
            FuelType = carEditProcessData.FuelType.FromGrpcMessage(),
            Image = carEditProcessData.ImageUrl,
            BigImages = carEditProcessData.BigImageUrls.ToArray(),
            AdditionalCarOptionsJson = JsonSerializer.Serialize(additionalCarOptions),
        };
    }

    public static Car ToCar(this CarEditProcessData carEditProcessData)
    {
        return new Car
        {
            Brand = carEditProcessData.Brand,
            Model = carEditProcessData.Model,
            Count = carEditProcessData.Count,
            FuelType = carEditProcessData.FuelType,
            PriceForStandartConfiguration = carEditProcessData.Price,
            CorpusType = carEditProcessData.CorpusType,
            Color = carEditProcessData.Color,
            EngineCapacity = carEditProcessData.EngineCapacity,
            ImageUrl = carEditProcessData.Image,
            BigImageURLs = carEditProcessData.BigImages,
            AdditionalCarOptions = carEditProcessData.AdditionalCarOptions.ToList()
        };
    }
}