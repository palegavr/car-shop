using CarShop.CarStorage.Database;
using CarShop.CarStorage.Database.Entities;
using Microsoft.EntityFrameworkCore;

namespace CarShop.CarStorage.Repositories;

public class CarConfigurationsRepository(CarStorageDatabase _db)
{
    public async Task AddAsync(CarConfiguration carConfiguration)
    {
        _db.CarConfigurations.Add(carConfiguration);
        await _db.SaveChangesAsync();
        _db.Entry(carConfiguration).State = EntityState.Detached;
    }

    public async Task<CarConfiguration?> GetByIdAsync(Guid carConfigurationId)
    {
        return await _db.CarConfigurations
            .AsNoTracking()
            .SingleOrDefaultAsync(c => c.Id == carConfigurationId);
    }

    public async Task<CarConfiguration[]> GetForCarAsync(long carId)
    {
        CarConfiguration[] configurations = await _db.CarConfigurations
            .AsNoTracking()
            .Where(c => c.CarId == carId).ToArrayAsync();
        
        return configurations;
    }
    
    public async Task UpdateAsync(CarConfiguration carConfiguration)
    {
        _db.CarConfigurations.Update(carConfiguration);
        await _db.SaveChangesAsync();
        _db.Entry(carConfiguration).State = EntityState.Detached;
    }
}