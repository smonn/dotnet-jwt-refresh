using System.Threading.Tasks;
using JwtRefresh.Models;
using MongoDB.Bson;
using MongoDB.Driver;

namespace JwtRefresh.Repositories
{
    public class RefreshTokenRepository : RepositoryBase<RefreshToken>, IRefreshTokenRepository
    {
        public RefreshTokenRepository(IMongoDatabase database) : base(database, "RefreshTokens")
        {
        }

        public async Task<RefreshToken> FindByAccountIdAsync(ObjectId accountId)
        {
            var filter = Builders<RefreshToken>.Filter.And(
                Builders<RefreshToken>.Filter.Eq(x => x.AccountId, accountId),
                Builders<RefreshToken>.Filter.Eq(x => x.IsRevoked, false)
            );
            return await _collection.Find(filter).FirstOrDefaultAsync();
        }

        public async Task<RefreshToken> FindByValueAsync(string value)
        {
            var filter = Builders<RefreshToken>.Filter.And(
                Builders<RefreshToken>.Filter.Eq(x => x.Value, value),
                Builders<RefreshToken>.Filter.Eq(x => x.IsRevoked, false)
            );
            return await _collection.Find(filter).FirstOrDefaultAsync();
        }

        protected override UpdateDefinition<RefreshToken> BuildUpdateDefinition(RefreshToken model)
        {
            return Builders<RefreshToken>.Update
                .Set(x => x.LastUsed, model.LastUsed)
                .Set(x => x.Value, model.Value)
                .Set(x => x.IsRevoked, model.IsRevoked);
        }
    }
}
