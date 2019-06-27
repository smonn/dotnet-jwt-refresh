using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using JwtRefresh.Api.ViewModels;
using JwtRefresh.Models;
using JwtRefresh.Repositories;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MongoDB.Driver;

namespace JwtRefresh.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class AccountsController : ControllerBase
    {
        private readonly IAccountRepository _accountRepository;

        public AccountsController(IAccountRepository accountRepository)
        {
            _accountRepository = accountRepository;
        }

        [HttpPost]
        [AllowAnonymous]
        public async Task<ActionResult<Account>> Post([FromBody] CreateAccountRequest request)
        {
            try
            {
                var account = new Account
                {
                    Username = request.Username,
                    Password = BCrypt.Net.BCrypt.EnhancedHashPassword(request.Password),
                };
                await _accountRepository.CreateAsync(account);
                var response = new CreateAccountResponse
                {
                    Id = account.Id,
                    Username = account.Username,
                };
                return Ok(new { Account = response });
            }
            catch (MongoWriteException)
            {
                return BadRequest(new { Error = "Username is already in use." });
            }
        }
    }
}
