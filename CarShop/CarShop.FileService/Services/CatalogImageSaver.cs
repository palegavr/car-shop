using Microsoft.OpenApi.Expressions;

namespace CarShop.FileService.Services;

public class CatalogImageSaver
{
    private static readonly string PATH_TO_SAVE = 
        Path.Combine(Directory.GetCurrentDirectory(),
        $"wwwroot/images/route/catalog");
    public async Task<string> SaveImageAsync(byte[] imageBytes, string fileExtention)
    {
        if (!fileExtention.StartsWith("."))
        {
            fileExtention = "." + fileExtention;
        }
        
        if (!Directory.Exists(PATH_TO_SAVE))
        {
            Directory.CreateDirectory(PATH_TO_SAVE);
        }

        Guid imageGuid = Guid.NewGuid();
        while(Directory.GetFiles(PATH_TO_SAVE).Contains(imageGuid.ToString()))
        {
            imageGuid = Guid.NewGuid();
        }

        string imagePath = Path.Combine(PATH_TO_SAVE, imageGuid + fileExtention)
            .Replace('\\', '/');
        using (var stream = new FileStream(imagePath, FileMode.Create))
        {
            await stream.WriteAsync(imageBytes, 0, imageBytes.Length); 
        }

        return imagePath;
    }
}