using System.Threading.Tasks;
using JwtRefresh.Services.Login;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace JwtRefresh.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class LoginController : ControllerBase
    {
        private readonly ILoginService _loginService;

        public LoginController(ILoginService loginService)
        {
            _loginService = loginService;
        }

        [AllowAnonymous]
        [HttpPost]
        public async Task<ActionResult<LoginResponse>> Post([FromBody] LoginRequest request)
        {
            var response = await _loginService.LoginAsync(request);
            if (response.Error == null)
            {
                return Ok(response);
            }
            return BadRequest(response);
        }

        [AllowAnonymous]
        [HttpPost("refresh")]
        public async Task<ActionResult<LoginResponse>> Refresh([FromBody] RefreshTokenRequest request)
        {
            var response = await _loginService.RefreshAsync(request);
            if (response.Error == null)
            {
                return Ok(response);
            }
            return BadRequest(response);
        }
    }
}
