using CarShop.CarStorage.Database;
using CarShop.ServiceDefaults.CommonTypes;
using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;

namespace CarShop.CarStorage.Repositories
{
    public class CarsRepository(CarStorageDatabase _db)
    {
        public async Task AddCarAsync(Car carForAdding)
        {
            _db.Cars.Add(carForAdding);
            await _db.SaveChangesAsync();
        }

        public async Task UpdateCarAsync(Car carWithNewData)
        {
            _db.Cars.Update(carWithNewData);
            await _db.SaveChangesAsync();
        }

        public async Task DeleteCarAsync(long carId)
        {
            Car car = new Car { Id = carId, IsDeleted = true };
            _db.Entry(car).Property(e => e.IsDeleted).IsModified = true;
            car.IsDeleted = true;
            await _db.SaveChangesAsync();
        }

        public async Task<GetCarsResult> GetCarsAsync<TKey>(
            Range? fromTo = null,
            Expression<Func<Car, bool>>? searchBy = null,
            Expression<Func<Car, TKey>>? orderBy = null,
            bool desc = false)
        {
            IQueryable<Car>? query = null;

            if (searchBy is not null)
                query = query?.Where(searchBy) ?? _db.Cars.Where(searchBy);

            if (orderBy is not null)
                if (desc)
                    query = query?.OrderByDescending(orderBy) ?? _db.Cars.OrderByDescending(orderBy);
                else
                    query = query?.OrderBy(orderBy) ?? _db.Cars.OrderBy(orderBy);

            long totalResultsCount = query?.Count() ?? _db.Cars.Count();

            if (fromTo is not null)
                query = query?.Take(fromTo.Value) ?? _db.Cars.Take(fromTo.Value);

            return new GetCarsResult
            {
                Cars = await (query?.ToArrayAsync() ?? _db.Cars.ToArrayAsync()),
                TotalResultsCount = totalResultsCount
            };
        }

        public async Task<Car?> GetCarByIdAsync(long id)
        {
            return await _db.Cars.SingleOrDefaultAsync(car => car.Id == id);
        }
    }

    public class GetCarsResult
    {
        public IEnumerable<Car> Cars { get; init; }
        public long TotalResultsCount { get; init; }
    }
}
