using System.Globalization;
using System.Text;
using Microsoft.Extensions.Primitives;

namespace CarShop.Web.Extensions;

public static class NumberExtensions
{
    private static readonly NumberFormatInfo NumberFormatInfo = new()
    {
        NumberGroupSeparator = " ",
        NumberGroupSizes = [3],
    };

    private const string NumberFormat = "#,0";

    public static string ToDisplayString(this int number, int numbersAfterDotCount = 15)
    {
        return number.ToString(PrepareNumberFormat(numbersAfterDotCount), NumberFormatInfo);
    }

    public static string ToDisplayString(this double number, int numbersAfterDotCount = 15)
    {
        return number.ToString(PrepareNumberFormat(numbersAfterDotCount), NumberFormatInfo);
    }

    public static string ToDisplayString(this float number, int numbersAfterDotCount = 15)
    {
        return number.ToString(PrepareNumberFormat(numbersAfterDotCount), NumberFormatInfo);
    }

    private static string PrepareNumberFormat(int numbersAfterDotCount)
    {
        ArgumentOutOfRangeException.ThrowIfNegative(numbersAfterDotCount);
        ArgumentOutOfRangeException.ThrowIfGreaterThan(numbersAfterDotCount, 15);
        
        var sb = new StringBuilder(numbersAfterDotCount + 1);
        sb.Append(".");
        for (int i = 0; i < numbersAfterDotCount; i++)
        {
            sb.Append("#");
        }

        return NumberFormat + sb;
    }
}