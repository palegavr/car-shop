using CarShop.CarStorage.Database.Entities;
using CarShop.CarStorage.Database.Entities.AdditionalCarOption;

namespace CarShop.CarStorage.Extensions;

public static class CarConfigurationExtensions
{
    public static CarShop.CarStorageService.Grpc.CarConfiguration 
        ToGrpcMessage(this CarConfiguration carConfiguration)
    {
        var carConfigurationMessage = new CarStorageService.Grpc.CarConfiguration()
        {
            Id = carConfiguration.Id.ToString(),
            CarId = carConfiguration.CarId,
            AirConditioner = carConfiguration.AirConditioner,
            HeatedDriversSeat = carConfiguration.HeatedDriversSeat,
            SeatHeightAdjustment = carConfiguration.SeatHeightAdjustment,
            IsAvailable = carConfiguration.IsAvaliable,
        };
        if (carConfiguration.DifferentCarColor is not null)
        {
            carConfigurationMessage.DifferentCarColor = carConfiguration.DifferentCarColor;
        }
        return carConfigurationMessage;
    }

    public static CarConfiguration
        FromGrpcMessage(this CarShop.CarStorageService.Grpc.CarConfiguration carConfiguration)
    {
        return new()
        {
            Id = Guid.Parse(carConfiguration.Id),
            CarId = carConfiguration.CarId,
            AirConditioner = carConfiguration.AirConditioner,
            HeatedDriversSeat = carConfiguration.HeatedDriversSeat,
            SeatHeightAdjustment = carConfiguration.SeatHeightAdjustment,
            DifferentCarColor = carConfiguration.HasDifferentCarColor
                ? carConfiguration.DifferentCarColor
                : null,
            IsAvaliable = carConfiguration.IsAvailable,
        };
    }

    public static bool IsCorrectBy(
        this CarConfiguration carConfiguration,
        IEnumerable<AdditionalCarOption> additionalCarOptions)
    {
        var availableTypesSet = new HashSet<AdditionalCarOptionType>(additionalCarOptions.Select(option => option.Type));

        var requiredOptions = new[]
        {
            (carConfiguration.AirConditioner, AdditionalCarOptionType.AirConditioner),
            (carConfiguration.HeatedDriversSeat, AdditionalCarOptionType.HeatedDriversSeat),
            (carConfiguration.SeatHeightAdjustment, AdditionalCarOptionType.SeatHeightAdjustment),
            (carConfiguration.DifferentCarColor is not null, AdditionalCarOptionType.DifferentCarColor)
        };

        return requiredOptions
            .All(option => !option.Item1 || availableTypesSet.Contains(option.Item2));
    }
}
