using CarShop.CarStorage.Database;
using CarShop.ServiceDefaults.CommonTypes;
using CarShop.ServiceDefaults.ServiceInterfaces.CarStorage;
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
            _db.Entry(carForAdding).State = EntityState.Detached;
        }

        public async Task UpdateCarAsync(Car carWithNewData)
        {
            _db.Cars.Update(carWithNewData);
            await _db.SaveChangesAsync();
            _db.Entry(carWithNewData).State = EntityState.Detached;
        }

        public async Task UpdateCarAsync(long id, UpdateCarRequest updateCarRequest)
        {
            Car car = new Car { Id = id };
            updateCarRequest.GetType().GetProperties()
                .Where(prop => prop.GetValue(updateCarRequest) is not null)
                .ToList().ForEach(updateCarRequestPropery =>
                {
                    var carProperty = car.GetType()
                    .GetProperties()
                    .Where(p => p.Name == updateCarRequestPropery.Name)
                    .Single();

                    carProperty.SetValue(car, updateCarRequestPropery.GetValue(updateCarRequest));

                    _db.Entry(car).Property(carProperty.Name).IsModified = true;
                });

            await _db.SaveChangesAsync();
            _db.Entry(car).State = EntityState.Detached;
        }

        public async Task DeleteCarAsync(long carId)
        {
            Car notDeletedCar = new Car { Id = carId };
            _db.Cars.Remove(notDeletedCar);
            await _db.SaveChangesAsync();
            _db.Entry(notDeletedCar).State = EntityState.Detached;
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

            var result = new GetCarsResult
            {
                Cars = await (query?.ToArrayAsync() ?? _db.Cars.ToArrayAsync()),
                TotalResultsCount = totalResultsCount
            };

            foreach (var car in result.Cars)
            {
                _db.Entry(car).State = EntityState.Detached;
            }

            return result;
        }

        public async Task<Car?> GetCarByIdAsync(long id)
        {
            Car? car = await _db.Cars
                .SingleOrDefaultAsync(car => car.Id == id);

            if (car is not null)
                _db.Entry(car).State = EntityState.Detached;

            return car;
        }
    }

    public class GetCarsResult
    {
        public IEnumerable<Car> Cars { get; init; }
        public long TotalResultsCount { get; init; }
    }
}
