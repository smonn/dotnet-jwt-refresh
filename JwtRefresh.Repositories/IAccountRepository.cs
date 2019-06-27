using System.Threading.Tasks;
using JwtRefresh.Models;

namespace JwtRefresh.Repositories
{
    public interface IAccountRepository : IRepository<Account>
    {
        Task<Account> FindByUsernameAsync(string username);
    }
}
