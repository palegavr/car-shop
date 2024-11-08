
using CarShop.AdminService.Database;
using CarShop.AdminService.Grpc;
using CarShop.AdminService.Repositories;
using CarShop.AdminService.Services;
using Microsoft.EntityFrameworkCore;

namespace CarShop.AdminService;

public class Program
{
    public static void Main(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);
        
        builder.Services.AddGrpc();

        builder.Services.AddDbContext<AdminServiceDatabase>();
        builder.Services.AddScoped<AdminsRepository>();
        builder.Services.AddScoped<RefreshSessionsRepository>();
        builder.Services.AddSingleton<TokensPairGenerator>();

        var app = builder.Build();

		WaitUntilMigrationDone().Wait();

		app.MapGrpcService<AdminServiceImpl>();

        app.Run();
    }

	private static async Task WaitUntilMigrationDone()
	{
		while (true)
		{
			try
			{
				using (var db = new AdminServiceDatabase())
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
