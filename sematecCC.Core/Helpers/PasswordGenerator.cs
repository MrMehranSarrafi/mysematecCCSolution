using System.Security.Cryptography;

namespace SematecCC.Core;

internal class PasswordGenerator
{
}


public class OtpService
{
    public string GenerateOtp(int length = 5)
    {
        var otp = new char[length];
        using (var rng = RandomNumberGenerator.Create())
        {
            byte[] bytes = new byte[length];
            rng.GetBytes(bytes);
            for (int i = 0; i < length; i++)
            {
                otp[i] = (char)('0' + (bytes[i] % 10)); // فقط اعداد 0-9
            }
        }
        return new string(otp);
    }
}
