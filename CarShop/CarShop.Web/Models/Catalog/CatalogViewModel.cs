using CarShop.CarStorageService.Grpc;

namespace CarShop.Web.Models.Catalog
{
	public class CatalogViewModel
	{
		public IEnumerable<Car> Cars { get; set; } = [];
        public bool IsSearchResultsPage { get; set; } = false;
		public int CurrentPage { get; set; } = 1;
		public int PagesCount { get; set; } = 1;
		public GetCarsRequest GetCarsOptions { get; set; } = null!;
	}
}
