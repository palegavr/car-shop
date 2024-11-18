using System.ComponentModel.DataAnnotations;

namespace CarShop.ServiceDefaults.Attributes.Validation;

public class EnumValidationAttribute : ValidationAttribute
{
    private readonly Type _enumType;

    public EnumValidationAttribute(Type enumType)
    {
        if (!enumType.IsEnum)
        {
            throw new ArgumentException("Type must be an enum.");
        }
        _enumType = enumType;
    }

    protected override ValidationResult? IsValid(object? value, ValidationContext validationContext)
    {
        if (value == null)
        {
            return ValidationResult.Success;
        }

        if (!Enum.IsDefined(_enumType, value))
        {
            return new ValidationResult($"{validationContext.DisplayName} has an invalid value.");
        }

        return ValidationResult.Success;
    }
}