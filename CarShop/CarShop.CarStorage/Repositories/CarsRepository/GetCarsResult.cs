using CarShop.CarStorage.Database.Entities;
using CarShop.CarStorage.Database.Entities.Car;

namespace CarShop.CarStorage.Repositories.CarsRepository
{
    public class GetCarsResult
    {
        public IEnumerable<Car> Cars { get; init; }
        public int TotalResultsCount { get; init; }
    }
}