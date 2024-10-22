using CarShop.CarStorage.Database;
using CarShop.ServiceDefaults.ServiceInterfaces.CarStorage;
using Microsoft.EntityFrameworkCore;

namespace CarShop.CarStorage.Repositories;

public class AdditionalCarOptionsRepository(CarStorageDatabase _db)
{
    public async Task AddAdditionalCarOptionAsync(AdditionalCarOption additionalCarOption)
    {
        _db.AdditionalCarOptions.Add(additionalCarOption);
        await _db.SaveChangesAsync();
        _db.Entry(additionalCarOption).State = EntityState.Detached;
    }
    
    public async Task UpdateAdditionalCarOptionAsync(AdditionalCarOption additionalCarOption)
    {
        _db.AdditionalCarOptions.Update(additionalCarOption);
        await _db.SaveChangesAsync();
        _db.Entry(additionalCarOption).State = EntityState.Detached;
    }
    
    public async Task DeleteAdditionalCarOptionAsync(long id)
    {
        AdditionalCarOption additionalCarOption = new AdditionalCarOption { Id = id };
        _db.AdditionalCarOptions.Remove(additionalCarOption);
        await _db.SaveChangesAsync();
    }

    public async Task<AdditionalCarOption[]> GetAdditionalCarOptionsForCar(long carId)
    {
        AdditionalCarOption[] additionalCarOptions = 
            await _db.AdditionalCarOptions.Where(option => option.CarId == carId).ToArrayAsync();
        
        foreach (var additionalCarOption in additionalCarOptions)
        {
            _db.Entry(additionalCarOption).State = EntityState.Detached;
        }
        
        return additionalCarOptions;
    }
}