using CarShop.CarStorage.Database.Entities.Car;

namespace CarShop.CarStorage.Repositories.CarsRepository
{
    public class GetCarsOptions
    {
        private int? _startIndex = null;
        private int? _endIndex = null;
        public int? StartIndex
		{
			get => _startIndex;
            set => _startIndex = (value is null || value < 0) ? null : value;
        }
		public int? EndIndex 
        { 
            get => _endIndex;
            set => _endIndex = (value is null || value < 0) ? null : value;
        }
        public string? Brand { get; set; } = null;
        public double? MinimumEngineCapacity { get; set; } = null;
        public double? MaximumEngineCapacity { get; set; } = null;
        public FuelType? FuelType { get; set; } = null;
        public CorpusType? CorpusType { get; set; } = null;
        public double? MinimumPrice { get; set; } = null;
        public double? MaximumPrice { get; set; } = null;
        public SortType? SortType { get; set; } = null;
        public SortBy? SortBy { get; set; } = null;
    }
}