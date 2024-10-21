using CarShop.ServiceDefaults;
using CarShop.ServiceDefaults.ServiceInterfaces.AdminService;
using CarShop.ServiceDefaults.ServiceInterfaces.CarStorage;
using CarShop.Web.ModelBuilders;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;

namespace CarShop.Web;

public class Program
{
    public static void Main(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);
        builder.AddServiceDefaults();

        builder.Services.AddControllersWithViews(options =>
        {
            options.ModelBinderProviders.Insert(0, new DoubleModelBinderProvider());
        });

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

        app.MapDefaultEndpoints();

        // Configure the HTTP request pipeline.
        /*if (!app.Environment.IsDevelopment() || true)
        {
            app.UseExceptionHandler("/Error/NotFound404");
            app.UseStatusCodePagesWithReExecute("/Error/{0}");

            // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
            app.UseHsts();
        }*/

        //app.UseHttpsRedirection();
        app.UseStatusCodePagesWithReExecute("/Error/{0}");
        app.UseStaticFiles();

        app.UseRouting();

        app.UseAuthorization();

        app.MapControllerRoute(
            name: "default",
            pattern: "{controller=Home}/{action=Index}");


        app.Run();
    }
}