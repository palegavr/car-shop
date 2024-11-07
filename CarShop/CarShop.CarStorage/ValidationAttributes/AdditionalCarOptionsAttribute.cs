using System.ComponentModel.DataAnnotations;
using CarShop.CarStorage.Database.Entities.AdditionalCarOption;
using CarShop.CarStorage.Database.Entities.Car;
using Microsoft.OpenApi.Extensions;

namespace CarShop.CarStorage.ValidationAttributes;

[AttributeUsage(AttributeTargets.Property | AttributeTargets.Field)]
public class AdditionalCarOptionsAttribute : ValidationAttribute
{
    protected override ValidationResult? IsValid(object? value, ValidationContext validationContext)
    {
        IEnumerable<AdditionalCarOption>? options =
            validationContext.ObjectInstance.GetType()
                .GetProperty(validationContext.MemberName!)
                ?.GetValue(validationContext.ObjectInstance) as IEnumerable<AdditionalCarOption>;
        
        foreach (var option in options)
        {
            if (!Validator.TryValidateObject(option, new(option), null, true) ||
                !Enum.IsDefined(option.Type))
            {
                return new ValidationResult($"Type: {option.Type} not valid.");
            }
        }
        
        if (options == null)
        {
            return new ValidationResult("Invalid object.");
        }
        
        if (!options.Any())
        {
            return ValidationResult.Success;
        }
        
        if (options // Если есть элементы с одинаковым типом
            .GroupBy(option => option.Type)
            .Any(group => group.Count() > 1))
        {
            return new ValidationResult("Есть элементы с одинаковым типом.");
        }
        
        return ValidationResult.Success;
    }
}