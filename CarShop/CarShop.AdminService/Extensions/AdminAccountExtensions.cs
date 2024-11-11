using CarShop.AdminService.Database.Entities;
using CarShop.AdminService.Grpc;

namespace CarShop.AdminService.Extensions;

public static class AdminAccountExtensions
{
    public static AdminAccount ToGrpcMessage(this Admin adminAccount)
    {
        return new AdminAccount
        {
            Id = adminAccount.Id,
            Email = adminAccount.Email,
            Password = adminAccount.Password,
            Priority = adminAccount.Priority,
            Roles = { adminAccount.Roles },
            Banned = adminAccount.Banned,
        };
    }
    
    public static Admin FromGrpcMessage(this AdminAccount adminAccount)
    {
        return new Admin
        {
            Id = adminAccount.Id,
            Email = adminAccount.Email,
            Password = adminAccount.Password,
            Priority = adminAccount.Priority,
            Roles = adminAccount.Roles.ToArray(),
            Banned = adminAccount.Banned,
        };
    }
}