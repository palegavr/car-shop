
using CarShop.AdminService.Grpc;

namespace CarShop.ApiGateway.Models;

public class GetAdminsPayload
{
    public GetAccountsRequest.Types.SortType? SortType { get; set; }
    public GetAccountsRequest.Types.SortBy? SortBy { get; set; }
    public int? MinPriority { get; set; }
    public int? MaxPriority { get; set; }
    public string[]? HaveRoles { get; set; }
    public bool? Banned { get; set; }
}