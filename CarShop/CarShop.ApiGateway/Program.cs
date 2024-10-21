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
                options.Events = new JwtBearerEvents
                    {
                        OnMessageReceived = async context =>
                        {
                            string? refreshToken = context.Request.Cookies["refresh_token"];
                            string? accessToken = context.Request.Cookies["access_token"];

                            if (refreshToken is null && accessToken is null)
                            {
                                return;
                            }
                            
                            if (refreshToken is null && accessToken is not null)
                            {
                                context.Response.Cookies.DeleteAccessTokenCookie();
                                return;
                            }

                            if (refreshToken is not null && accessToken is null)
                            {
                                AdminServiceClient adminServiceClient = context.HttpContext.RequestServices.GetService<AdminServiceClient>()!;
                                var responce = await adminServiceClient.UpdateTokensAsync(refreshToken);
                                if (responce.StatusCode == System.Net.HttpStatusCode.OK)
                                {
                                    TokensPairResponce tokensPairResponce
                                        = (await responce.Content.ReadFromJsonAsync<TokensPairResponce>())!;
                                    
                                    context.Response.Cookies.SetAccessTokenCookie(tokensPairResponce.AccessToken);
                                    context.Response.Cookies.SetRefreshTokenCookie(tokensPairResponce.RefreshToken);
                                    context.Token = tokensPairResponce.AccessToken;
                                }
                                else
                                {
                                    context.Response.Cookies.DeleteRefreshTokenCookie();
                                }
                            }

                            if (refreshToken is not null && accessToken is not null)
                            {
                                context.Token = accessToken;
                            }
                        },
                        OnAuthenticationFailed = async context =>
                        {
                            context.Response.Cookies.DeleteAccessTokenCookie();
                        }
                    };
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