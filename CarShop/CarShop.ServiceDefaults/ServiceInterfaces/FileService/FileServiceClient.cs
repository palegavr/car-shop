using System.Net.Http.Json;
using Microsoft.Extensions.Logging;

namespace CarShop.ServiceDefaults.ServiceInterfaces.FileService;

public class FileServiceClient(HttpClient _httpClient)
{
    public const string BASE_ADDRESS = "http://carshop.fileservice:8080";

    public async Task<string?> SaveCatalogImageAsync(
        SaveCatalogImageRequest request, CancellationToken cancellationToken = default)
    {
        var responce = 
            await _httpClient.PostAsJsonAsync("api/fileservice/save-catalog-image", request, cancellationToken);

        if (!responce.IsSuccessStatusCode)
        {
            return null;
        }
        
        return (await responce.Content.ReadFromJsonAsync<SaveCatalogImageResponse>(cancellationToken))!.PublicPath;
    }
    
    public static void ConfigureClient(HttpClient httpClient)
    {
        httpClient.BaseAddress = new Uri(BASE_ADDRESS);
    }
}