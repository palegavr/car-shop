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
        public async Task<GetCarsResult> GetCarsAsync(GetCarsOptions? getCarsOptions = null)
        {
            IQueryable<Car>? query = null;
            long totalResultsCount = query?.Count() ?? _db.Cars.Count();
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
        private IQueryable<Car>? MakeGetCarsQuery(GetCarsOptions getCarsOptions)
        {
            IQueryable<Car>? query = null;
            if (getCarsOptions.Brand is not null)
            {
                Expression<Func<Car, bool>> brandCondition =
                    car => car.Brand.Contains(getCarsOptions.Brand);
                query = query?.Where(brandCondition)
                    ?? _db.Cars.Where(brandCondition);
            }
            if (getCarsOptions.EngineCapacityFrom is not null)
            {
                Expression<Func<Car, bool>> engineCapacityFromCondition =
                    car => car.EngineCapacity >= getCarsOptions.EngineCapacityFrom;
                query = query?.Where(engineCapacityFromCondition)
                    ?? _db.Cars.Where(engineCapacityFromCondition);
            }
            if (getCarsOptions.EngineCapacityTo is not null)
            {
                Expression<Func<Car, bool>> engineCapacityToCondition =
                    car => car.EngineCapacity <= getCarsOptions.EngineCapacityTo;
                query = query?.Where(engineCapacityToCondition)
                    ?? _db.Cars.Where(engineCapacityToCondition);
            }
            if (getCarsOptions.FuelType is not null)
            {
                Expression<Func<Car, bool>> fuelTypeCondition =
                    car => car.FuelType == getCarsOptions.FuelType;
                query = query?.Where(fuelTypeCondition)
                    ?? _db.Cars.Where(fuelTypeCondition);
            }
            if (getCarsOptions.CorpusType is not null)
            {
                Expression<Func<Car, bool>> corpusTypeCondition =
                    car => car.CorpusType == getCarsOptions.CorpusType;
                query = query?.Where(corpusTypeCondition)
                    ?? _db.Cars.Where(corpusTypeCondition);
            }
            if (getCarsOptions.PriceFrom is not null)
            {
                Expression<Func<Car, bool>> priceFromCondition =
                    car => car.PriceForStandardConfiguration >= getCarsOptions.PriceFrom;
                query = query?.Where(priceFromCondition)
                    ?? _db.Cars.Where(priceFromCondition);
            }
            if (getCarsOptions.PriceTo is not null)
            {
                Expression<Func<Car, bool>> priceToCondition =
                    car => car.PriceForStandardConfiguration <= getCarsOptions.PriceTo;
                query = query?.Where(priceToCondition)
                    ?? _db.Cars.Where(priceToCondition);
            }

            return query;
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
}
