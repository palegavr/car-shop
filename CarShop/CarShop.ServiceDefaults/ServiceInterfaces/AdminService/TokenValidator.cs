using System.Net.Http.Json;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using static Microsoft.Extensions.DependencyInjection.ServiceProviderServiceExtensions;
using CarShop.AdminService.Grpc;

namespace CarShop.ServiceDefaults.ServiceInterfaces.AdminService
{
    using Microsoft.IdentityModel.Tokens;

    public static class TokenValidator
    {
        public static readonly TokenValidationParameters ValidationParameters = new()
        {
            ValidateIssuer = true,
            ValidIssuer = AuthOptions.ISSUER,

            ValidateAudience = true,
            ValidAudience = AuthOptions.AUDIENCE,

            ValidateLifetime = true,
            ClockSkew = TimeSpan.Zero,

            ValidateIssuerSigningKey = true,
            IssuerSigningKey = AuthOptions.GetSymmetricSecurityKey(),
        };

        public static readonly JwtBearerEvents CookieJwtBearerEvents = new JwtBearerEvents
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
                    var adminServiceClient =
                        context.HttpContext.RequestServices
                            .GetService<CarShop.AdminService.Grpc.AdminService.AdminServiceClient>()!;
                    
                    var response = await adminServiceClient.UpdateTokensAsync(new()
                    {
                        RefreshToken = refreshToken,
                    });

                    if (response.Result == UpdateTokensReply.Types.UpdateTokensResult.Success)
                    {
                        context.Response.Cookies.SetAccessTokenCookie(response.AccessToken);
                        context.Response.Cookies.SetRefreshTokenCookie(response.RefreshToken);
                        context.Token = response.AccessToken;
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
            OnAuthenticationFailed = async context => { context.Response.Cookies.DeleteAccessTokenCookie(); }
        };
    }
}