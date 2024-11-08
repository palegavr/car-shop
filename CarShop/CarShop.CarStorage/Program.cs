
using CarShop.CarStorage.Database;
using CarShop.CarStorage.Grpc;
using CarShop.CarStorage.Repositories;
using CarShop.CarStorage.Repositories.CarsRepository;
using Microsoft.EntityFrameworkCore;

namespace CarShop.CarStorage;

public class Program
{
    public static void Main(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);

        builder.Services.AddGrpc();

        builder.Services.AddDbContext<CarStorageDatabase>();
        builder.Services.AddScoped<CarsRepository>();
        builder.Services.AddScoped<CarEditProcessesRepository>();
        builder.Services.AddScoped<CarConfigurationsRepository>();

        var app = builder.Build();

        app.MapGrpcService<CarStorageServiceImpl>();

        WaitUntilMigrationDone().Wait();

        app.Run();
    }

    private static async Task WaitUntilMigrationDone()
    {
        while (true)
        {
            try
            {
                using (var db = new CarStorageDatabase())
		        {
			        db.Database.Migrate();
		        }
                break;
            }
            catch { }

            await Task.Delay(2000);
        }
		
	}
}
