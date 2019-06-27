using System;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using JwtRefresh.Models;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;

namespace JwtRefresh.Services.Utils
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

        public static string CreateAccessToken(Account account, IConfiguration configuration)
        {
            var secret = configuration["Auth:Secret"];
            var issuer = configuration["Auth:Issuer"];
            var audience = configuration["Auth:Audience"];
            var expires = DateTime.UtcNow.AddSeconds(Convert.ToDouble(configuration["Auth:ExpiresInSeconds"]));

            var tokenHandler = new JwtSecurityTokenHandler();
            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(new[] {
                        new Claim(ClaimTypes.NameIdentifier, account.Id.ToString()),
                        new Claim(ClaimTypes.Name, account.Username),
                    }),
                SigningCredentials = new SigningCredentials(
                    new SymmetricSecurityKey(Encoding.UTF8.GetBytes(secret)),
                    SecurityAlgorithms.HmacSha256
                ),
                Expires = expires,
                Issuer = issuer,
                Audience = audience,
            };

            var securityToken = tokenHandler.CreateToken(tokenDescriptor);
            return tokenHandler.WriteToken(securityToken);
        }

        public static string HashPassword(string password)
        {
            return BCrypt.Net.BCrypt.EnhancedHashPassword(password);
        }

        public static bool VerifyPassword(string password, string hash)
        {
            return BCrypt.Net.BCrypt.EnhancedVerify(password, hash);
        }

        public static ClaimsPrincipal GetClaimsPrincipalFromExpiredToken(string accessToken, IConfiguration configuration)
        {
            var secret = configuration["Auth:Secret"];
            var issuer = configuration["Auth:Issuer"];
            var audience = configuration["Auth:Audience"];
            var expires = DateTime.UtcNow.AddSeconds(Convert.ToDouble(configuration["Auth:ExpiresInSeconds"]));
            var tokenValidationParameters = new TokenValidationParameters
            {
                ValidateAudience = true,
                ValidAudience = audience,

                ValidateIssuer = true,
                ValidIssuer = issuer,

                ValidateIssuerSigningKey = true,
                IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(secret)),

                // ignore expiration date on token
                ValidateLifetime = false,
            };

            var tokenHandler = new JwtSecurityTokenHandler();
            var principal = tokenHandler.ValidateToken(accessToken, tokenValidationParameters, out var validatedToken);
            var jwtSecurityToken = validatedToken as JwtSecurityToken;
            if (jwtSecurityToken == null || !jwtSecurityToken.Header.Alg.Equals(SecurityAlgorithms.HmacSha256, StringComparison.InvariantCultureIgnoreCase))
            {
                throw new SecurityTokenException("Invalid token");
            }

            return principal;
        }
    }
}
