using System;
using MongoDB.Bson;

namespace JwtRefresh.Models
{
    public class RefreshToken : ModelBase
    {
        public ObjectId AccountId { get; set; }
        public string Value { get; set; }
        public bool IsRevoked { get; set; }
        public DateTime Expires { get; set; }
    }
}
