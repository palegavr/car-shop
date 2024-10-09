
using CarShop.AdminService.Database;
using CarShop.AdminService.Repositories;
using Microsoft.EntityFrameworkCore;

namespace CarShop.AdminService;

public class Program
{
    public static void Main(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);
        builder.AddServiceDefaults();

        // Add services to the container.

        builder.Services.AddControllers();
        // Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
        builder.Services.AddEndpointsApiExplorer();
        builder.Services.AddSwaggerGen();

        builder.Services.AddDbContext<AdminServiceDatabase>();
        builder.Services.AddScoped<AdminsRepository>();
        builder.Services.AddScoped<RefreshSessionsRepository>();

        var app = builder.Build();

        app.MapDefaultEndpoints();

        // Configure the HTTP request pipeline.
        if (app.Environment.IsDevelopment())
        {
            app.UseSwagger();
            app.UseSwaggerUI();
        }

        app.UseHttpsRedirection();

        app.UseAuthorization();

		WaitUntilMigrationDone().Wait();

		app.MapControllers();

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
