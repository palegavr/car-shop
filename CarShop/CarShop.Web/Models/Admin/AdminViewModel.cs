using CarShop.AdminService.Grpc;

namespace CarShop.Web.Models.Admin;

public class AdminViewModel
{
    public string HeadHtmlContent { get; set; }
    public string BodyHtmlContent { get; set; }
    public PerformingAdmin PerformingAdmin { get; set; }
    public AdminAccount[] AdminAccounts { get; set; }
    public string? AdminEmail { get; set; }
}