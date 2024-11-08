using CarShop.CarStorage.Database.Entities;
using CarShop.CarStorage.Database.Entities.AdditionalCarOption;

namespace CarShop.CarStorage.Extensions;

public static class AdditionalCarOptionTypeExtensions
{
    public static CarStorageService.Grpc.AdditionalCarOption.Types.Type
        ToGrpcMessage(this AdditionalCarOptionType additionalCarOptionType)
    {
        return (CarStorageService.Grpc.AdditionalCarOption.Types.Type)additionalCarOptionType;
    }
    
    public static AdditionalCarOptionType
        FromGrpcMessage(this CarStorageService.Grpc.AdditionalCarOption.Types.Type additionalCarOptionType)
    {
        return (AdditionalCarOptionType)additionalCarOptionType;
    }
}