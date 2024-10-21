using CarShop.CarStorage.Database;
using CarShop.ServiceDefaults.ServiceInterfaces.CarStorage;
using Microsoft.EntityFrameworkCore;

namespace CarShop.CarStorage.Repositories;

public class CarEditProcessesRepository(CarStorageDatabase _db)
{
    public async Task AddCarEditProcessAsync(CarEditProcess carEditProcess)
    {
        _db.CarEditProcesses.Add(carEditProcess);
        await _db.SaveChangesAsync();
        _db.Entry(carEditProcess).State = EntityState.Detached;
    }
    
    public async Task UpdateCarEditProcessAsync(CarEditProcess carEditProcess)
    {
        _db.CarEditProcesses.Update(carEditProcess);
        await _db.SaveChangesAsync();
        _db.Entry(carEditProcess).State = EntityState.Detached;
    }
    
    public async Task<CarEditProcess?> GetCarEditProcessByAdminIdAndCarId(long adminId, long carId)
    {
        CarEditProcess? carEditProcess = await _db.CarEditProcesses
            .SingleOrDefaultAsync(process => process.AdminId == adminId && process.CarId == carId);

        if (carEditProcess is not null)
        {
            _db.Entry(carEditProcess).State = EntityState.Detached;
        }
        
        return carEditProcess;
    }
}