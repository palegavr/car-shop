using System.ComponentModel.DataAnnotations;
using CarShop.CarStorage.Database;
using CarShop.CarStorage.Database.Entities;
using CarShop.CarStorage.Database.Entities.Car;
using CarShop.CarStorage.Database.Entities.CarEditProcess;
using CarShop.CarStorage.Extensions;
using Microsoft.EntityFrameworkCore;

namespace CarShop.CarStorage.Repositories;

public class CarEditProcessesRepository(CarStorageDatabase _db)
{
    public async Task AddAsync(CarEditProcess carEditProcess)
    {
        Car carForValidation = carEditProcess.Process.ToCar();
        Validator.ValidateObject(carForValidation, new(carForValidation), true);
        _db.CarEditProcesses.Add(carEditProcess);
        await _db.SaveChangesAsync();
        _db.Entry(carEditProcess).State = EntityState.Detached;
    }
    
    public async Task UpdateAsync(CarEditProcess carEditProcess)
    {
        Car carForValidation = carEditProcess.Process.ToCar();
        Validator.ValidateObject(carForValidation, new(carForValidation), true);
        _db.CarEditProcesses.Update(carEditProcess);
        await _db.SaveChangesAsync();
        _db.Entry(carEditProcess).State = EntityState.Detached;
    }
    
    public async Task DeleteAsync(long id)
    {
        CarEditProcess carEditProcess = new CarEditProcess { Id = id };
        _db.CarEditProcesses.Remove(carEditProcess);
        await _db.SaveChangesAsync();
    }
    
    public async Task<CarEditProcess?> GetByAdminIdAndCarIdAsync(long adminId, long carId)
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