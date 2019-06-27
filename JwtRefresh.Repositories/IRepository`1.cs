using System.Collections.Generic;
using System.Threading.Tasks;
using JwtRefresh.Models;
using MongoDB.Bson;
using MongoDB.Driver;
using MongoDB.Driver.Linq;

namespace JwtRefresh.Repositories
{
    public interface IRepository<TModel> where TModel : ModelBase
    {
        Task<TModel> CreateAsync(TModel model);
        Task<bool> UpdateAsync(ObjectId id, TModel model);
        Task<bool> DeleteAsync(ObjectId id);
        Task<TModel> FindByIdAsync(ObjectId id);
        Task<IList<TModel>> FindAsync(FilterDefinition<TModel> filter);
        IMongoQueryable AsQueryable();
    }
}
