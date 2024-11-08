using CarShop.CarStorage.Repositories.CarsRepository;
using CarShop.CarStorageService.Grpc;

namespace CarShop.CarStorage.Extensions;

public static class SortByExtensions
{
    public static GetCarsRequest.Types.SortBy ToGrpcMessage(this SortBy sortBy)
    {
        return (GetCarsRequest.Types.SortBy)sortBy;
    }
    
    public static SortBy FromGrpcMessage(this GetCarsRequest.Types.SortBy sortBy)
    {
        return (SortBy)sortBy;
    }
}