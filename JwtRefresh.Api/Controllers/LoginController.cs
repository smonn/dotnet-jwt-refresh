using System;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using JwtRefresh.Api.Extensions;
using JwtRefresh.Api.Utils;
using JwtRefresh.Api.ViewModels;
using JwtRefresh.Models;
using JwtRefresh.Repositories;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;

namespace JwtRefresh.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class LoginController : ControllerBase
    {
        private readonly IAccountRepository _accountRepository;
        private readonly IRefreshTokenRepository _refreshTokenRepository;
        private readonly IConfiguration _configuration;

        public LoginController(IAccountRepository accountRepository, IRefreshTokenRepository refreshTokenRepository, IConfiguration configuration)
        {
            _accountRepository = accountRepository;
            _refreshTokenRepository = refreshTokenRepository;
            _configuration = configuration;
        }

        [AllowAnonymous]
        [HttpPost]
        public async Task<ActionResult<LoginResponse>> Post([FromBody] LoginRequest request)
        {
            var account = await _accountRepository.FindByUsernameAsync(request.Username);
            if (account != null && BCrypt.Net.BCrypt.EnhancedVerify(request.Password, account.Password))
            {
                var accessToken = _createAccessToken(account);

                var refreshToken = new RefreshToken
                {
                    AccountId = account.Id,
                    LastUsed = DateTime.UtcNow,
                    Value = CryptoUtils.RandomString(),
                };
                await _refreshTokenRepository.CreateAsync(refreshToken);

                return Ok(new LoginResponse
                {
                    AccessToken = accessToken,
                    RefreshToken = refreshToken.Value,
                });
            }

            return BadRequest(new { Error = "Incorrect username and/or password" });
        }

        [AllowAnonymous]
        [HttpPost("refresh")]
        public async Task<ActionResult<LoginResponse>> Refresh([FromBody] RefreshTokenRequest request)
        {
            var token = await _refreshTokenRepository.FindByValueAsync(request.RefreshToken);
            if (token == null)
            {
                return Unauthorized(new { Error = "Invalid refresh token" });
            }

            var account = await _accountRepository.FindByIdAsync(token.AccountId);
            var accessToken = _createAccessToken(account);
            token.Value = CryptoUtils.RandomString();
            token.LastUsed = DateTime.UtcNow;
            await _refreshTokenRepository.UpdateAsync(token.Id, token);

            return Ok(new LoginResponse
            {
                AccessToken = accessToken,
                RefreshToken = token.Value,
            });
        }

        private string _createAccessToken(Account account)
        {
            var tokenHandler = new JwtSecurityTokenHandler();
            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(new[] {
                        new Claim(ClaimTypes.NameIdentifier, account.Id.ToString()),
                        new Claim(ClaimTypes.Name, account.Username),
                    }),
                SigningCredentials = new SigningCredentials(
                    new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["Auth:Secret"])),
                    SecurityAlgorithms.HmacSha256Signature
                ),
                Expires = DateTime.UtcNow.AddSeconds(30),
                Issuer = _configuration["Auth:Issuer"],
                Audience = _configuration["Auth:Audience"],
            };

            var securityToken = tokenHandler.CreateToken(tokenDescriptor);
            return tokenHandler.WriteToken(securityToken);
        }
    }
}
