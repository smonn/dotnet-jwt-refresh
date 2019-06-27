using MongoDB.Bson;

namespace JwtRefresh.Models
{
    public abstract class ModelBase
    {
        public ObjectId Id { get; set; }
    }
}
