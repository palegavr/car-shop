using System.Text.Json.Serialization;

namespace CarShop.Web.Models.Admin;

public class PerformingAdmin
{
    [JsonPropertyName("id")] public long Id { get; set; }
    [JsonPropertyName("email")] public string Email { get; set; }
    [JsonPropertyName("priority")] public int Priority { get; set; }
    [JsonPropertyName("roles")] public List<string> Roles { get; set; }
}