using CarShop.CarStorage.Database;
using CarShop.ServiceDefaults.ServiceInterfaces.CarStorage;
using Microsoft.EntityFrameworkCore;

namespace CarShop.CarStorage.Repositories;

public class CarConfigurationsRepository(CarStorageDatabase _db)
{
    public async Task AddCarConfiguration(CarConfiguration carConfiguration)
    {
        _db.CarConfigurations.Add(carConfiguration);
        await _db.SaveChangesAsync();
        _db.Entry(carConfiguration).State = EntityState.Detached;
    }

    public async Task<CarConfiguration?> GetCarConfigurationById(Guid carConfigurationId)
    {
        return await _db.CarConfigurations
            .AsNoTracking()
            .SingleOrDefaultAsync(c => c.Id == carConfigurationId);
    }

    public async Task<CarConfiguration[]> GetCarConfigurationsOfCar(long carId)
    {
        CarConfiguration[] configurations = await _db.CarConfigurations
            .AsNoTracking()
            .Where(c => c.CarId == carId).ToArrayAsync();
        
        return configurations;
    }
    
    public async Task UpdateCarConfigurationAsync(CarConfiguration carConfiguration)
    {
        _db.CarConfigurations.Update(carConfiguration);
        await _db.SaveChangesAsync();
        _db.Entry(carConfiguration).State = EntityState.Detached;
    }
}