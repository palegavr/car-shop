using System.ComponentModel.DataAnnotations;

namespace CarShop.ServiceDefaults.ServiceInterfaces.FileService;

public class SaveCatalogImageRequest
{
    [Required]
    public byte[] ImageBytes { get; set; }
    [Required]
    [RegularExpression("^(\\.)?(png|jpg|jpeg)$")]
    public string FileExtention { get; set; }
}