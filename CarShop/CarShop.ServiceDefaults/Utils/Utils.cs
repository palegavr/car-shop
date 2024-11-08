using System.Net.Mail;
using System.Security.Claims;
using Microsoft.AspNetCore.Mvc;

namespace CarShop.ServiceDefaults.Utils;

public static class Utils
{
    public static long? GetAdminIdFromClaimsPrincipal(ClaimsPrincipal claimsPrincipal)
    {
        return long.TryParse(claimsPrincipal.Claims.FirstOrDefault(claim => claim.Type == "id")?.Value ?? "NotNumber",
            out long adminId) ? adminId : null;
    }

    public static bool IsEmail(string email)
    {
        try
        {
            var mailAddress = new MailAddress(email);
            return true;
        }
        catch
        {
            return false;
        }
    }
}