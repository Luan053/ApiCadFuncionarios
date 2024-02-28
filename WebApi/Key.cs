using System;
using System.Security.Cryptography;

namespace WebApi
{
    public class Key
    {
        public static string Secret = GenerateRandomKey();

        private static string GenerateRandomKey()
        {
            const int keySizeInBytes = 32; // 256 bits

            using (var rng = new RNGCryptoServiceProvider())
            {
                byte[] keyBytes = new byte[keySizeInBytes];
                rng.GetBytes(keyBytes);

                return BitConverter.ToString(keyBytes).Replace("-", string.Empty).ToLower();
            }
        }
    }
}
