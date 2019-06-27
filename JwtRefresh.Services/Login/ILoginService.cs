using System.Threading.Tasks;

namespace JwtRefresh.Services.Login
{
    public interface ILoginService
    {
        Task<LoginResponse> LoginAsync(LoginRequest request);
        Task<LoginResponse> RefreshAsync(RefreshTokenRequest request);
    }
}
