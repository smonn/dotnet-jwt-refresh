using System;
using System.Threading.Tasks;
using JwtRefresh.Models;
using JwtRefresh.Repositories;
using JwtRefresh.Services.Extensions;
using JwtRefresh.Services.Utils;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace JwtRefresh.Services.Login
{
    public class LoginService : ILoginService
    {
        private readonly IAccountRepository _accountRepository;
        private readonly IRefreshTokenRepository _refreshTokenRepository;
        private readonly IConfiguration _configuration;
        private readonly ILogger<LoginService> _logger;

        public LoginService(IAccountRepository accountRepository, IRefreshTokenRepository refreshTokenRepository, IConfiguration configuration, ILogger<LoginService> logger)
        {
            _accountRepository = accountRepository;
            _refreshTokenRepository = refreshTokenRepository;
            _configuration = configuration;
            _logger = logger;
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
                        Expires = DateTime.UtcNow.AddDays(Convert.ToDouble(_configuration["Auth:RefreshTokenExpiresInDays"])),
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
            catch (Exception ex)
            {
                // swallow for now
                _logger.LogError(ex, "Failed to authenticate user");
            }

            return new LoginResponse { Error = "Invalid username and/or password" };
        }

        public async Task<LoginResponse> RefreshAsync(RefreshTokenRequest request)
        {
            try
            {
                var user = CryptoUtils.GetClaimsPrincipalFromExpiredToken(request.AccessToken, _configuration);
                var accountId = user.GetAccountId();

                var token = await _refreshTokenRepository.FindByAccountIdAndValueAsync(accountId, request.RefreshToken);
                if (token == null)
                {
                    return new LoginResponse { Error = "Invalid access token and/or refresh token" };
                }

                var account = await _accountRepository.FindByIdAsync(token.AccountId);
                var accessToken = CryptoUtils.CreateAccessToken(account, _configuration);
                token.Value = CryptoUtils.RandomString();
                token.Expires = DateTime.UtcNow.AddDays(Convert.ToDouble(_configuration["Auth:RefreshTokenExpiresInDays"]));
                await _refreshTokenRepository.UpdateAsync(token.Id, token);

                return new LoginResponse
                {
                    AccessToken = accessToken,
                    RefreshToken = token.Value,
                };
            }
            catch (Exception ex)
            {
                // swallow for now
                _logger.LogError(ex, "Failed to process refresh token");
            }

            return new LoginResponse { Error = "Invalid access token and/or refresh token" };
        }
    }
}
