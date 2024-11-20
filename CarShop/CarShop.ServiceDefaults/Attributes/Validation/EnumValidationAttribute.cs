using System;
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

        // Проверяем, что значение имеет корректный тип
        if (!Enum.IsDefined(_enumType, value) && !_enumType.IsDefined(typeof(FlagsAttribute), false))
        {
            return new ValidationResult($"{validationContext.DisplayName} has an invalid value.");
        }

        if (_enumType.IsDefined(typeof(FlagsAttribute), false))
        {
            // Для флагов: проверяем, что все установленные биты соответствуют допустимым значениям
            long longValue = Convert.ToInt64(value);
            long allFlags = 0;

            foreach (var enumValue in Enum.GetValues(_enumType))
            {
                allFlags |= Convert.ToInt64(enumValue);
            }

            if ((longValue & ~allFlags) != 0)
            {
                return new ValidationResult($"{validationContext.DisplayName} has an invalid combination of flags.");
            }
        }
        else
        {
            // Для обычных enum: проверяем наличие значения в определении
            if (!Enum.IsDefined(_enumType, value))
            {
                return new ValidationResult($"{validationContext.DisplayName} has an invalid value.");
            }
        }

        return ValidationResult.Success;
    }
}