using System.Text.Json.Serialization;
using Microsoft.AspNetCore.Mvc;

namespace CarShop.ServiceDefaults.ServiceInterfaces.AdminService;

public class TokensPairResponce
{
    [JsonPropertyName("refresh_token")]
    public string RefreshToken { get; init; }
    
    [JsonPropertyName("access_token")]
    public string AccessToken { get; init; }
}