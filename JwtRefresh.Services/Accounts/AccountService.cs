using System.Threading.Tasks;
using JwtRefresh.Repositories;
using JwtRefresh.Models;
using MongoDB.Driver;
using JwtRefresh.Services.Utils;
using Microsoft.Extensions.Logging;
using System;

namespace JwtRefresh.Services.Accounts
{
    public class AccountService : IAccountService
    {
        private readonly IAccountRepository _accountRepository;
        private readonly ILogger<AccountService> _logger;

        public AccountService(IAccountRepository accountRepository, ILogger<AccountService> logger)
        {
            _accountRepository = accountRepository;
            _logger = logger;
        }

        public async Task<GetAccountResponse> GetAsync(GetAccountRequest request)
        {
            try
            {
                var account = await _accountRepository.FindByIdAsync(request.AccountId);
                if (account != null)
                {
                    return new GetAccountResponse
                    {
                        Account = new AccountPublic
                        {
                            EmailAddress = account.EmailAddress,
                            GivenName = account.GivenName,
                            Surname = account.Surname,
                            Username = account.Username,
                        },
                    };
                }
            }
            catch (Exception ex)
            {
                // swallow for now
                _logger.LogError(ex, "Failed to get account by id");
            }

            return new GetAccountResponse { Error = "Could not retrieve account details" };
        }

        public async Task<CreateAccountResponse> RegisterAsync(CreateAccountRequest request)
        {
            try
            {
                var account = new Account
                {
                    Username = request.Username,
                    Password = CryptoUtils.HashPassword(request.Password),
                    GivenName = request.GivenName,
                    Surname = request.Surname,
                    EmailAddress = request.EmailAddress,
                };
                await _accountRepository.CreateAsync(account);
                var response = new CreateAccountResponse
                {
                    Account = new AccountPublic
                    {
                        Username = account.Username,
                        EmailAddress = account.EmailAddress,
                        GivenName = account.GivenName,
                        Surname = account.Surname,
                    },
                };
                return response;
            }
            catch (MongoWriteException)
            {
                return new CreateAccountResponse { Error = "Username is already in use" };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to create account");
            }

            return new CreateAccountResponse { Error = "Failed to create account" };
        }
    }
}
