
using CarShop.CarStorage.Database.Entities;
using CarShop.CarStorage.Database.Entities.Car;

namespace CarShop.CarStorage.Extensions;

public static class CorpusTypeExtensions
{
    public static CarStorageService.Grpc.Car.Types.CorpusType ToGrpcMessage(this CorpusType corpusType)
    {
        return (CarStorageService.Grpc.Car.Types.CorpusType)corpusType;
    }
    
    public static CorpusType FromGrpcMessage(this CarStorageService.Grpc.Car.Types.CorpusType corpusType)
    {
        return (CorpusType)corpusType;
    }
}