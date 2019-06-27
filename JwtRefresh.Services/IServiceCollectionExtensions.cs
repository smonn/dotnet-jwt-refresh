using JwtRefresh.Services.Login;
using Microsoft.Extensions.DependencyInjection;

namespace JwtRefresh.Services
{
    public static class IServiceCollectionExtensions
    {
        public static IServiceCollection AddServices(this IServiceCollection services)
        {
            services.AddScoped<ILoginService, LoginService>();
            return services;
        }
    }
}
