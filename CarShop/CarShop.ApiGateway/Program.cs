using CarShop.ServiceDefaults;
using CarShop.ServiceDefaults.ServiceInterfaces.AdminService;
using CarShop.ServiceDefaults.ServiceInterfaces.CarStorage;
using CarShop.ServiceDefaults.ServiceInterfaces.FileService;
using Microsoft.AspNetCore.Authentication.JwtBearer;

namespace CarShop.ApiGateway;

public class Program
{
    public static void Main(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);

        // Add services to the container.
        builder.Services.AddAuthorization();

        // Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
        builder.Services.AddEndpointsApiExplorer();
        builder.Services.AddSwaggerGen();
        builder.Services.AddControllers();
        builder.Services.AddHttpClient<FileServiceClient>(FileServiceClient.ConfigureClient);
        builder.Services.AddHttpClient<CarStorageClient>(CarStorageClient.ConfigureClient);
        builder.Services.AddHttpClient<AdminServiceClient>(AdminServiceClient.ConfigureClient);
        builder.Services.AddAuthorization();
        builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
            .AddJwtBearer(options =>
            {
                options.TokenValidationParameters = TokenValidator.ValidationParameters;
                options.Events = TokenValidator.CookieJwtBearerEvents;
            });
        var app = builder.Build();
        app.UseAuthentication();
        app.UseAuthorization();
        // Configure the HTTP request pipeline.
        if (app.Environment.IsDevelopment())
        {
            app.UseSwagger();
            app.UseSwaggerUI();
        }
        
        app.MapControllers();
        app.Run();
    }
}