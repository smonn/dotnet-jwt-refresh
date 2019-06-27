using System;
using System.Threading.Tasks;
using JwtRefresh.Models;
using MongoDB.Driver;

namespace JwtRefresh.Repositories
{
    public class AccountRepository : RepositoryBase<Account>, IAccountRepository
    {
        public AccountRepository(IMongoDatabase database)
            : base(database, "accounts")
        {
        }

        public async Task<Account> FindByUsernameAsync(string username)
        {
            var filter = Builders<Account>.Filter.Eq(x => x.Username, username);
            var account = await _collection.Find(filter).FirstOrDefaultAsync();
            return account;
        }

        protected override UpdateDefinition<Account> BuildUpdateDefinition(Account model)
        {
            return Builders<Account>.Update
                .Set(x => x.Username, model.Username)
                .Set(x => x.Password, model.Password);
        }
    }
}
