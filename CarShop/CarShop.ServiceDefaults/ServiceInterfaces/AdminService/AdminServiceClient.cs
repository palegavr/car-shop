using System.Net.Http.Json;

namespace CarShop.ServiceDefaults.ServiceInterfaces.AdminService;

public class AdminServiceClient(HttpClient _httpClient)
{
    public const string BASE_ADDRESS = "http://carshop.adminservice:8080";

    public async Task<HttpResponseMessage> LoginAsync(string login, string password, CancellationToken cancellationToken = default)
    {
        return await _httpClient.PostAsJsonAsync("api/admin/login", new LoginRequest
        {
            Email = login,
            Password = password
        }, cancellationToken);
    }

    public async Task<HttpResponseMessage> UpdateTokensAsync(string refreshToken,
        CancellationToken cancellationToken = default)
    {
        return await _httpClient.PostAsJsonAsync("api/admin/updateTokens", new UpdateTokensRequest
            {
                RefreshToken = refreshToken
            }, cancellationToken);
    }
    
    public static void ConfigureClient(HttpClient httpClient)
    {
        httpClient.BaseAddress = new Uri(BASE_ADDRESS);
    }
}