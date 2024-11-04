namespace CarShop.AdminService;

public static class Constants
{
    public static readonly string[] DefaultAdminRoles = [
        "admin.account.change-password.own",
        "admin.account.ban.own"
    ];

    public const int HighestAdminPriority = 1;
    public const int LowestAdminPriority = 2000000000;
}