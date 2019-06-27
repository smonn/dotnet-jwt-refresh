using System.Threading.Tasks;
using JwtRefresh.Models;
using MongoDB.Bson;

namespace JwtRefresh.Repositories
{
    public interface IRefreshTokenRepository : IRepository<RefreshToken>
    {
        Task<RefreshToken> FindByValueAsync(string value);
        Task<RefreshToken> FindByAccountIdAsync(ObjectId accountId);
    }
}
