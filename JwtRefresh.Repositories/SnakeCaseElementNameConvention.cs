using MongoDB.Bson.Serialization;
using MongoDB.Bson.Serialization.Conventions;

namespace JwtRefresh.Repositories
{
    public class SnakeCaseElementNameConvention : ConventionBase, IMemberMapConvention
    {
        public void Apply(BsonMemberMap memberMap)
        {
            memberMap.SetElementName(memberMap.MemberName.ToSnakeCase());
        }
    }
}
