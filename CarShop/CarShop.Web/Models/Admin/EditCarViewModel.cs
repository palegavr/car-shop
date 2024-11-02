namespace CarShop.Web.Models.Admin;

public class EditCarViewModel
{
    public string HeadHtmlContent { get; set; }
    public string BodyHtmlContent { get; set; }
    public string ProcessDataInDbJsonEncoded { get; set; }
    public string CurrentProcessDataJsonEncoded { get; set; }
    public long CarId { get; set; }
}