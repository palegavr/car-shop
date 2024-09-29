using CarShop.CarStorage.Database;
using CarShop.ServiceDefaults.CommonTypes;
using CarShop.ServiceDefaults.ServiceInterfaces.CarStorage;
using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;
using System.Linq.Dynamic.Core;
using System.Linq;

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
            IQueryable<Car>? query = getCarsOptions is not null ? MakeGetCarsQuery(getCarsOptions) : null;
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
                    car => car.Brand.ToLower().Contains(getCarsOptions.Brand.ToLower());
                query = query?.Where(brandCondition)
                    ?? _db.Cars.Where(brandCondition);
            }
            if (getCarsOptions.MinimumEngineCapacity is not null)
            {
                Expression<Func<Car, bool>> engineCapacityFromCondition =
                    car => car.EngineCapacity >= getCarsOptions.MinimumEngineCapacity;
                query = query?.Where(engineCapacityFromCondition)
                    ?? _db.Cars.Where(engineCapacityFromCondition);
            }
            if (getCarsOptions.MaximumEngineCapacity is not null)
            {
                Expression<Func<Car, bool>> engineCapacityToCondition =
                    car => car.EngineCapacity <= getCarsOptions.MaximumEngineCapacity;
                query = query?.Where(engineCapacityToCondition)
                    ?? _db.Cars.Where(engineCapacityToCondition);
            }
            if (getCarsOptions.FuelType is not null)
            {
                Expression<Func<Car, bool>> fuelTypeCondition =
                    car => car.FuelType.HasFlag(getCarsOptions.FuelType);
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
            if (getCarsOptions.MinimumPrice is not null)
            {
                Expression<Func<Car, bool>> priceFromCondition =
                    car => car.PriceForStandartConfiguration >= getCarsOptions.MinimumPrice;
                query = query?.Where(priceFromCondition)
                    ?? _db.Cars.Where(priceFromCondition);
            }
            if (getCarsOptions.MaximumPrice is not null)
            {
                Expression<Func<Car, bool>> priceToCondition =
                    car => car.PriceForStandartConfiguration <= getCarsOptions.MaximumPrice;
                query = query?.Where(priceToCondition)
                    ?? _db.Cars.Where(priceToCondition);
            }
			if (getCarsOptions.SortBy is not null)
			{
                bool desc = getCarsOptions.SortType.HasValue && getCarsOptions.SortType == SortType.Descending;
                string orderExpression = $"{getCarsOptions.SortBy.ToString()}";
                if (desc) orderExpression += " descending";

				query = query?.OrderBy(orderExpression) 
                    ?? _db.Cars.OrderBy(orderExpression);
			}
            if (getCarsOptions.StartIndex is not null)
            {
                query = query?.Skip(getCarsOptions.StartIndex.Value)
                    ?? _db.Cars.Skip(getCarsOptions.StartIndex.Value);
            }
			if (getCarsOptions.EndIndex is not null)
			{
                int takeCount = getCarsOptions.EndIndex.Value - (getCarsOptions.StartIndex ?? 0);
				query = query?.Take(takeCount)
					?? _db.Cars.Take(takeCount);
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
