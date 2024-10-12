using CarShop.ServiceDefaults.ServiceInterfaces.AdminService;

namespace CarShop.Web;

public static class Extentions
{

    public static void SetRefreshTokenCookie(this IResponseCookies cookies, string refreshToken)
    {
        var cookieOptionsRefresh = new CookieOptions
        {
            HttpOnly = true,
            SameSite = SameSiteMode.Strict,
            MaxAge = AuthOptions.REFRESH_TOKEN_LIFETIME
        };
        
        cookies.Append("refresh_token", refreshToken, cookieOptionsRefresh);
    }
    
    public static void SetAccessTokenCookie(this IResponseCookies cookies, string accessToken)
    {
        var cookieOptionsAccess = new CookieOptions
        {
            HttpOnly = true,
            SameSite = SameSiteMode.Strict,
            MaxAge = AuthOptions.ACCESS_TOKEN_LIFETIME
        };
        
        cookies.Append("access_token", accessToken, cookieOptionsAccess);
    }

    public static void DeleteRefreshTokenCookie(this IResponseCookies cookies)
    {
        cookies.Delete("refresh_token");
    }
    
    public static void DeleteAccessTokenCookie(this IResponseCookies cookies)
    {
        cookies.Delete("access_token");
    }
}