using CarShop.ServiceDefaults.CommonTypes;

namespace CarShop.Web.Models
{
	public class CatalogViewModel
	{
		public IEnumerable<Car> Cars { get; set; } = [];
        public bool IsSearchResultsPage { get; set; } = false;
    }
}
