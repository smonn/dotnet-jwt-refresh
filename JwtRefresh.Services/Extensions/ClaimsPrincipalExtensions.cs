using System.Security.Claims;
using MongoDB.Bson;

namespace JwtRefresh.Services.Extensions
{
    public static class ClaimsPrincipalExtensions
    {
        public static ObjectId GetAccountId(this ClaimsPrincipal user)
        {
            var claimValue = user.FindFirstValue(ClaimTypes.NameIdentifier);
            if (ObjectId.TryParse(claimValue, out var accountId))
            {
                return accountId;
            }
            return ObjectId.Empty;
        }
    }
}
