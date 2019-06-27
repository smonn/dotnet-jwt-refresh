using System.Threading.Tasks;

namespace JwtRefresh.Services.Accounts
{
    public interface IAccountService
    {
        Task<CreateAccountResponse> RegisterAsync(CreateAccountRequest request);
        Task<GetAccountResponse> GetAsync(GetAccountRequest request);
    }
}
