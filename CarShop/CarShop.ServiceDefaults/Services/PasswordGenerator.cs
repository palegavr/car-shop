using System.Security.Cryptography;
using System.Text;

namespace CarShop.ServiceDefaults.Services;

public class PasswordGenerator
{
    private const string AllowedChars = "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789!@#$%^&*()";

    public string GeneratePassword(int length)
    {
        ArgumentOutOfRangeException.ThrowIfNegativeOrZero(length, nameof(length));

        var password = new StringBuilder(length);
        using (var rng = RandomNumberGenerator.Create())
        {
            byte[] randomNumber = new byte[1];
            for (int i = 0; i < length; i++)
            {
                rng.GetBytes(randomNumber);
                int pos = randomNumber[0] % AllowedChars.Length;
                password.Append(AllowedChars[pos]);
            }
        }

        return password.ToString();
    }
}