using System.Threading.Tasks;
using JwtRefresh.Models;
using MongoDB.Bson;

namespace JwtRefresh.Repositories
{
    public interface IRefreshTokenRepository : IRepository<RefreshToken>
    {
        Task<RefreshToken> FindByAccountIdAndValueAsync(ObjectId accountId, string value);
    }
}
