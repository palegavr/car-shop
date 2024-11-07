using CarShop.CarStorage.Repositories.CarsRepository;
using CarShop.CarStorageService.Grpc;

namespace CarShop.CarStorage.Extensions;

public static class SortTypeExtensions
{
    public static GetCarsRequest.Types.SortType ToGrpcMessage(this SortType sortType)
    {
        return (GetCarsRequest.Types.SortType)sortType;
    }
    
    public static SortType FromGrpcMessage(this GetCarsRequest.Types.SortType sortType)
    {
        return (SortType)sortType;
    }
}