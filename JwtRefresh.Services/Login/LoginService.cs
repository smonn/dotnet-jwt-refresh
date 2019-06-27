using System;
using System.Threading.Tasks;
using JwtRefresh.Models;
using JwtRefresh.Repositories;
using JwtRefresh.Services.Utils;
using Microsoft.Extensions.Configuration;

namespace JwtRefresh.Services.Login
{
    public class LoginService : ILoginService
    {
        private readonly IAccountRepository _accountRepository;
        private readonly IRefreshTokenRepository _refreshTokenRepository;
        private readonly IConfiguration _configuration;

        public LoginService(IAccountRepository accountRepository, IRefreshTokenRepository refreshTokenRepository, IConfiguration configuration)
        {
            _accountRepository = accountRepository;
            _refreshTokenRepository = refreshTokenRepository;
            _configuration = configuration;
        }

        public async Task<LoginResponse> LoginAsync(LoginRequest request)
        {
            try
            {
                var account = await _accountRepository.FindByUsernameAsync(request.Username);
                if (account != null && CryptoUtils.VerifyPassword(request.Password, account.Password))
                {
                    var accessToken = CryptoUtils.CreateAccessToken(account, _configuration);
                    var refreshToken = new RefreshToken
                    {
                        AccountId = account.Id,
                        LastUsed = DateTime.UtcNow,
                        Value = CryptoUtils.RandomString(),
                    };
                    await _refreshTokenRepository.CreateAsync(refreshToken);
                    return new LoginResponse
                    {
                        AccessToken = accessToken,
                        RefreshToken = refreshToken.Value,
                    };
                }
            }
            catch
            {
                // swallow for now
            }

            return new LoginResponse { Error = "Invalid username and/or password" };
        }

        public async Task<LoginResponse> RefreshAsync(RefreshTokenRequest request)
        {
            try
            {
                var token = await _refreshTokenRepository.FindByValueAsync(request.RefreshToken);
                if (token == null)
                {
                    return new LoginResponse { Error = "Invalid refresh token" };
                }

                var account = await _accountRepository.FindByIdAsync(token.AccountId);
                var accessToken = CryptoUtils.CreateAccessToken(account, _configuration);
                token.Value = CryptoUtils.RandomString();
                token.LastUsed = DateTime.UtcNow;
                await _refreshTokenRepository.UpdateAsync(token.Id, token);

                return new LoginResponse
                {
                    AccessToken = accessToken,
                    RefreshToken = token.Value,
                };
            }
            catch
            {
                // swallow for now
            }

            return new LoginResponse { Error = "Invalid refresh token" };
        }
    }
}
