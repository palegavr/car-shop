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
            Id = !string.IsNullOrWhiteSpace(carConfiguration.Id)
                ? Guid.Parse(carConfiguration.Id)
                : Guid.Empty,
            CarId = carConfiguration.CarId,
            AirConditioner = carConfiguration.AirConditioner,
            HeatedDriversSeat = carConfiguration.HeatedDriversSeat,
            SeatHeightAdjustment = carConfiguration.SeatHeightAdjustment,
            DifferentCarColor = carConfiguration.HasDifferentCarColor
                ? carConfiguration.DifferentCarColor
                : null,
        };
    }

    public static bool IsCorrectBy(
        this CarConfiguration carConfiguration,
        IEnumerable<AdditionalCarOption> additionalCarOptions)
    {
        var availableTypesSet =
            new HashSet<AdditionalCarOptionType>(additionalCarOptions.Select(option => option.Type));

        var optionTuples = new[]
        {
            (carConfiguration.AirConditioner, AdditionalCarOptionType.AirConditioner),
            (carConfiguration.HeatedDriversSeat, AdditionalCarOptionType.HeatedDriversSeat),
            (carConfiguration.SeatHeightAdjustment, AdditionalCarOptionType.SeatHeightAdjustment),
            (carConfiguration.DifferentCarColor is not null, AdditionalCarOptionType.DifferentCarColor)
        };

        bool result = true;
        
        foreach (var optionTuple in optionTuples)
        {
            if ((optionTuple.Item1 &&
                 !availableTypesSet.Contains(optionTuple.Item2)) ||
                (!optionTuple.Item1 &&
                 additionalCarOptions
                     .SingleOrDefault(option => option.Type == optionTuple.Item2)?.IsRequired == true))
            {
                result = false;
            }
        }

        return result;
    }
}