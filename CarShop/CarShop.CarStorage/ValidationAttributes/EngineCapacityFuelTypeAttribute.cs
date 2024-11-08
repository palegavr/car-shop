using System.ComponentModel.DataAnnotations;
using CarShop.CarStorage.Database.Entities.Car;

namespace CarShop.CarStorage.ValidationAttributes;

public class EngineCapacityFuelTypeAttribute : ValidationAttribute
{
    protected override ValidationResult? IsValid(object? value, ValidationContext validationContext)
    {
        var car = validationContext.ObjectInstance as Car;
        if (car == null)
        {
            return new ValidationResult("Invalid object.");
        }

        if (car.EngineCapacity == 0 && car.FuelType != FuelType.Electric)
        {
            return new ValidationResult("EngineCapacity может быть 0, только если FuelType - электрика.");
        }

        return ValidationResult.Success;
    }
}