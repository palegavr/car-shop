using System.Text.RegularExpressions;
using CarShop.FileService.Services;
using Grpc.Core;

namespace CarShop.FileService.Grpc;

public class FileServiceImpl
    (CatalogImageSaver _catalogImageSaver): FileService.FileServiceBase
{
    public override async Task<SaveCatalogImageReply> SaveCatalogImage(SaveCatalogImageRequest request, ServerCallContext context)
    {
        if (!Regex.IsMatch(request.FileExtention, "^(\\.)?(png|jpg|jpeg)$"))
        {
            return new SaveCatalogImageReply
            {
                Result = SaveCatalogImageResult.UnacceptableFileExtention
            };
        }
        
        var publicPath = (await _catalogImageSaver.SaveImageAsync(
            request.ImageBytes.ToByteArray(),
            request.FileExtention)).Split("wwwroot", 2)[1];

        return new SaveCatalogImageReply
        {
            Result = SaveCatalogImageResult.Success,
            PublicPath = publicPath
        };
    }
}