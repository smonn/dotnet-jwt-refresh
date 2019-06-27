using System;
using System.Threading.Tasks;
using JwtRefresh.Models;
using MongoDB.Bson;
using MongoDB.Driver;

namespace JwtRefresh.Repositories
{
    public class RefreshTokenRepository : RepositoryBase<RefreshToken>, IRefreshTokenRepository
    {
        public RefreshTokenRepository(IMongoDatabase database) : base(database, "refresh_tokens")
        {
        }

        public async Task<RefreshToken> FindByAccountIdAndValueAsync(ObjectId accountId, string value)
        {
            var filter = Builders<RefreshToken>.Filter.And(
                Builders<RefreshToken>.Filter.Eq(x => x.AccountId, accountId),
                Builders<RefreshToken>.Filter.Eq(x => x.Value, value),
                Builders<RefreshToken>.Filter.Eq(x => x.IsRevoked, false),
                Builders<RefreshToken>.Filter.Gt(x => x.Expires, DateTime.UtcNow)
            );
            return await _collection.Find(filter).FirstOrDefaultAsync();
        }

        protected override UpdateDefinition<RefreshToken> BuildUpdateDefinition(RefreshToken model)
        {
            return Builders<RefreshToken>.Update
                .Set(x => x.Expires, model.Expires)
                .Set(x => x.Value, model.Value)
                .Set(x => x.IsRevoked, model.IsRevoked);
        }
    }
}
