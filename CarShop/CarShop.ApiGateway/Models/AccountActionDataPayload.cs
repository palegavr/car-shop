using System.Text.Json.Serialization;

namespace CarShop.ApiGateway.Models;

public class AccountActionDataPayload
{
    [JsonPropertyName("email")] public string? Email { get; set; }
    [JsonPropertyName("password")] public string? Password { get; set; }
    [JsonPropertyName("oldPassword")] public string? OldPassword { get; set; }
    [JsonPropertyName("role")] public string? Role { get; set; }
    [JsonPropertyName("priority")] public int? Priority { get; set; }
}