using System.Net.Mime;
using CarShop.ServiceDefaults.ServiceInterfaces.FileService;
using Google.Protobuf.Compiler;
using Microsoft.AspNetCore.Mvc;

namespace CarShop.ApiGateway.Controllers;

[ApiController]
[Route("api/[controller]")]
public class AdminController
    (FileServiceClient _fileServiceClient): ControllerBase
{
    [HttpPost]
    [Route("uploadimage")]
    [Consumes(MediaTypeNames.Multipart.FormData)]
    public async Task<IActionResult> UploadImageAsync([FromForm(Name = "images")] IFormFileCollection formFiles)
    {
        List<string> publicPathes = new(formFiles.Count);
        foreach (IFormFile formFile in formFiles)
        {
            byte[] buffer = new byte [formFile.Length];
            if ((await formFile.OpenReadStream().ReadAsync(buffer, 0, buffer.Length)) == buffer.Length)
            {
                var publicPath = await _fileServiceClient.SaveCatalogImageAsync(new SaveCatalogImageRequest
                {
                    ImageBytes = buffer,
                    FileExtention = Path.GetExtension(formFile.FileName)
                });

                if (publicPath is not null)
                {
                    publicPathes.Add(publicPath);
                }
            }
        }
        
        return Ok(publicPathes);
    }
}