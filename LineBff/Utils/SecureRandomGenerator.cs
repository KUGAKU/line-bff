using System.Security.Cryptography;

namespace LineBff.Utils
{
    public static class SecureRandomGenerator
    {
        public static string GenerateRandomString(int length)
        {
            const string allowedChars = "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789";
            byte[] randomBytes = new byte[length];

            using var rng = RandomNumberGenerator.Create();
            rng.GetBytes(randomBytes);

            char[] chars = new char[length];
            int allowedCharCount = allowedChars.Length;

            for (int i = 0; i < length; i++)
            {
                int randomByte = randomBytes[i] % allowedCharCount;
                chars[i] = allowedChars[randomByte];
            }

            return new string(chars);
        }
    }
}

