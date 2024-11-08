using CarShop.CarStorage.Database.Entities;
using CarShop.CarStorage.Database.Entities.AdditionalCarOption;

namespace CarShop.CarStorage.Extensions;

public static class AdditionalCarOptionExtensions
{
    public static CarStorageService.Grpc.AdditionalCarOption
        ToGrpcMessage(this AdditionalCarOption additionalCarOption)
    {
        return new()
        {
            Price = additionalCarOption.Price,
            Type = additionalCarOption.Type.ToGrpcMessage(),
            IsRequired = additionalCarOption.IsRequired,
        };
    }
    
    public static AdditionalCarOption
        FromGrpcMessage(this CarStorageService.Grpc.AdditionalCarOption additionalCarOption)
    {
        return new()
        {
            Price = additionalCarOption.Price,
            Type = additionalCarOption.Type.FromGrpcMessage(),
            IsRequired = additionalCarOption.IsRequired,
        };
    }

    public static IEnumerable<CarStorageService.Grpc.AdditionalCarOption>
        ToGrpcMessage(this IEnumerable<AdditionalCarOption> additionalCarOption)
    {
        return from option in additionalCarOption select option.ToGrpcMessage();
    }
    
    public static IEnumerable<AdditionalCarOption>
        FromGrpcMessage(this IEnumerable<CarStorageService.Grpc.AdditionalCarOption> additionalCarOption)
    {
        return from option in additionalCarOption select option.FromGrpcMessage();
    }
}