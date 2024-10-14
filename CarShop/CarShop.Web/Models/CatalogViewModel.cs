using CarShop.ServiceDefaults.ServiceInterfaces.CarStorage;

namespace CarShop.Web.Models
{
	public class CatalogViewModel
	{
		public IEnumerable<Car> Cars { get; set; } = [];
        public bool IsSearchResultsPage { get; set; } = false;
		public int CurrentPage { get; set; } = 1;
		public int PagesCount { get; set; } = 1;
        public GetCarsOptions GetCarsOptions { get; set; }
    }
}
