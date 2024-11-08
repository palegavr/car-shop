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

	public static IHostApplicationBuilder AddServiceDefaults(this IHostApplicationBuilder builder)
    {
        builder.ConfigureOpenTelemetry();

        builder.AddDefaultHealthChecks();

        builder.Services.AddServiceDiscovery();

        builder.Services.ConfigureHttpClientDefaults(http =>
        {
            // Turn on resilience by default
            http.AddStandardResilienceHandler();

            // Turn on service discovery by default
            http.AddServiceDiscovery();
        });

        return builder;
    }

    public static IHostApplicationBuilder ConfigureOpenTelemetry(this IHostApplicationBuilder builder)
    {
        builder.Logging.AddOpenTelemetry(logging =>
        {
            logging.IncludeFormattedMessage = true;
            logging.IncludeScopes = true;
        });

        builder.Services.AddOpenTelemetry()
            .WithMetrics(metrics =>
            {
                metrics.AddAspNetCoreInstrumentation()
                    .AddHttpClientInstrumentation()
                    .AddRuntimeInstrumentation();
            })
            .WithTracing(tracing =>
            {
                tracing.AddAspNetCoreInstrumentation()
                    // Uncomment the following line to enable gRPC instrumentation (requires the OpenTelemetry.Instrumentation.GrpcNetClient package)
                    //.AddGrpcClientInstrumentation()
                    .AddHttpClientInstrumentation();
            });

        builder.AddOpenTelemetryExporters();

        return builder;
    }

    private static IHostApplicationBuilder AddOpenTelemetryExporters(this IHostApplicationBuilder builder)
    {
        var useOtlpExporter = !string.IsNullOrWhiteSpace(builder.Configuration["OTEL_EXPORTER_OTLP_ENDPOINT"]);

        if (useOtlpExporter)
        {
            builder.Services.AddOpenTelemetry().UseOtlpExporter();
        }

        // Uncomment the following lines to enable the Azure Monitor exporter (requires the Azure.Monitor.OpenTelemetry.AspNetCore package)
        //if (!string.IsNullOrEmpty(builder.Configuration["APPLICATIONINSIGHTS_CONNECTION_STRING"]))
        //{
        //    builder.Services.AddOpenTelemetry()
        //       .UseAzureMonitor();
        //}

        return builder;
    }

    public static IHostApplicationBuilder AddDefaultHealthChecks(this IHostApplicationBuilder builder)
    {
        builder.Services.AddHealthChecks()
            // Add a default liveness check to ensure app is responsive
            .AddCheck("self", () => HealthCheckResult.Healthy(), ["live"]);

        return builder;
    }

    public static WebApplication MapDefaultEndpoints(this WebApplication app)
    {
        // Adding health checks endpoints to applications in non-development environments has security implications.
        // See https://aka.ms/dotnet/aspire/healthchecks for details before enabling these endpoints in non-development environments.
        if (app.Environment.IsDevelopment())
        {
            // All health checks must pass for app to be considered ready to accept traffic after starting
            app.MapHealthChecks("/health");

            // Only health checks tagged with the "live" tag must pass for app to be considered alive
            app.MapHealthChecks("/alive", new HealthCheckOptions
            {
                Predicate = r => r.Tags.Contains("live")
            });
        }

        return app;
    }
}
