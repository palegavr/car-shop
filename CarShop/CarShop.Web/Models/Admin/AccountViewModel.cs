using System.Text.Json.Serialization;

namespace CarShop.Web.Models.Admin;

public class AccountViewModel
{
    public string HeadHtmlContent { get; set; }
    public string BodyHtmlContent { get; set; }
    public Admin Administrator { get; set; }
    public PerformingAdmin PerformingAdministrator { get; set; }

    public class Admin
    {
        [JsonPropertyName("id")] public long Id { get; set; }
        [JsonPropertyName("priority")] public int Priority { get; set; }
        [JsonPropertyName("banned")] public bool Banned { get; set; }
        [JsonPropertyName("roles")] public List<string> Roles { get; set; }
    }
}