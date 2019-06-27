using System;
using System.Security.Cryptography;

namespace JwtRefresh.Api.Utils
{
    public static class CryptoUtils
    {
        public static string RandomString(int size = 32)
        {
            using (var rng = new RNGCryptoServiceProvider())
            {
                var tokenData = new byte[size];
                rng.GetBytes(tokenData);
                return Convert.ToBase64String(tokenData);
            }
        }
    }
}
