using CarShop.CarStorageService.Grpc;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Diagnostics.HealthChecks;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using Microsoft.Extensions.Logging;
using OpenTelemetry;
using OpenTelemetry.Metrics;
using OpenTelemetry.Trace;

namespace Microsoft.Extensions.Hosting;

// Adds common .NET Aspire services: service discovery, resilience, health checks, and OpenTelemetry.
// This project should be referenced by each service project in your solution.
// To learn more about using this project, see https://aka.ms/dotnet/aspire/service-defaults
public static class Extensions
{
    public static string ToDisplayString(this Car.Types.CorpusType corpusType)
    {
        return corpusType switch
        {
            Car.Types.CorpusType.Sedan => "Седан",
            Car.Types.CorpusType.Hatchback => "Хэтчбек",
        };
    }
    
    public static string ToDisplayString(this Car.Types.FuelType fuelType)
    {
        var fuelTypesDictionary = new Dictionary<Car.Types.FuelType, Func<string>>
        {
            { Car.Types.FuelType.Petrol, () => SingleFuelTypeToDisplayString(Car.Types.FuelType.Petrol) },
            { Car.Types.FuelType.Diesel, () => SingleFuelTypeToDisplayString(Car.Types.FuelType.Diesel) },
            { Car.Types.FuelType.Gas, () => SingleFuelTypeToDisplayString(Car.Types.FuelType.Gas) },
            { Car.Types.FuelType.Electric, () => SingleFuelTypeToDisplayString(Car.Types.FuelType.Electric) }
        };

        return string.Join(", ", fuelTypesDictionary
            .Where(kv => fuelType.HasFlag(kv.Key))
            .Select(kv => kv.Value()));
    }

    private static string SingleFuelTypeToDisplayString(Car.Types.FuelType fuelType)
    {
        return fuelType switch
        {
            Car.Types.FuelType.Diesel => "Дизель",
            Car.Types.FuelType.Petrol => "Бензин",
            Car.Types.FuelType.Gas => "Газ",
            Car.Types.FuelType.Electric => "Электрика",
            _ => string.Empty,
        };
    }
    
    public static string ToDisplayString(this GetCarsRequest.Types.SortBy sortBy)
    {
        return sortBy switch
        {
            GetCarsRequest.Types.SortBy.Brand => "Марка",
            GetCarsRequest.Types.SortBy.EngineCapacity => "Объём двигателя",
            GetCarsRequest.Types.SortBy.FuelType => "Вид топлива",
            GetCarsRequest.Types.SortBy.CorpusType => "Вид корпуса",
            GetCarsRequest.Types.SortBy.PriceForStandardConfiguration => "Цена"
        };
    }

	public static string ToDisplayString(this GetCarsRequest.Types.SortType sortType)
	{
		return sortType switch
		{
			GetCarsRequest.Types.SortType.Ascending => "По возрастанию",
            GetCarsRequest.Types.SortType.Descending => "По убыванию"
		};
	}
}
