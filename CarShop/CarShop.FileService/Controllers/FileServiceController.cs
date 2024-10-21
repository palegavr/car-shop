using CarShop.FileService.Services;
using CarShop.ServiceDefaults.ServiceInterfaces.FileService;
using Microsoft.AspNetCore.Mvc;

namespace CarShop.FileService.Controllers;

[Route("api/[controller]")]
[ApiController]
[Consumes("application/json")]
public class FileServiceController
    (CatalogImageSaver _catalogImageSaver): ControllerBase
{
    [HttpPost]
    [Route("save-catalog-image")]
    public async Task<IActionResult> SaveCatalogImageAsync([FromBody] SaveCatalogImageRequest saveCatalogImageRequest)
    {
        if (!ModelState.IsValid)
        {
            return BadRequest();
        }
        
        var publicPath = (await _catalogImageSaver.SaveImageAsync(
            saveCatalogImageRequest.ImageBytes,
            saveCatalogImageRequest.FileExtention)).Split("wwwroot", 2)[1];
        
        return Ok(new SaveCatalogImageResponse
        {
            PublicPath = publicPath
        });
    }
}