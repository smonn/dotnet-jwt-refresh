using System.Collections.Generic;
using System.Threading.Tasks;
using JwtRefresh.Models;
using MongoDB.Bson;
using MongoDB.Driver;
using MongoDB.Driver.Linq;

namespace JwtRefresh.Repositories
{
    public abstract class RepositoryBase<TModel> : IRepository<TModel> where TModel : ModelBase
    {
        protected IMongoDatabase _database { get; }
        public IMongoCollection<TModel> _collection { get; }

        public RepositoryBase(IMongoDatabase database, string collectionName)
        {
            _database = database;
            _collection = database.GetCollection<TModel>(collectionName);
        }

        public IMongoQueryable AsQueryable()
        {
            return _collection.AsQueryable();
        }

        public async Task<TModel> CreateAsync(TModel model)
        {
            await _collection.InsertOneAsync(model);
            return model;
        }

        public async Task<bool> DeleteAsync(ObjectId id)
        {
            var result = await _collection.DeleteOneAsync(_buildIdFilter(id));
            return result.DeletedCount == 1;
        }

        public async Task<IList<TModel>> FindAsync(FilterDefinition<TModel> filter)
        {
            return await _collection.Find(filter).ToListAsync();
        }

        public async Task<TModel> FindByIdAsync(ObjectId id)
        {
            return await _collection.Find(_buildIdFilter(id)).FirstOrDefaultAsync();
        }

        public async Task<bool> UpdateAsync(ObjectId id, TModel model)
        {
            var result = await _collection.UpdateOneAsync(_buildIdFilter(id), BuildUpdateDefinition(model));
            return result.MatchedCount == 1;
        }

        protected abstract UpdateDefinition<TModel> BuildUpdateDefinition(TModel model);

        private FilterDefinition<TModel> _buildIdFilter(ObjectId id)
        {
            return Builders<TModel>.Filter.Eq(x => x.Id, id);
        }
    }
}
