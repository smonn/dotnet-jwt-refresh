using System.Linq;

namespace JwtRefresh.Repositories
{
    public static class StringExtensions
    {
        public static string ToSnakeCase(this string source)
        {
            return string.Concat(
                source.Select(
                    (c, i) =>
                    {
                        if (i > 0 && char.IsUpper(c))
                        {
                            return $"_{c}";
                        }
                        return $"{c}";
                    }
                )
            ).ToLower();
        }
    }
}
