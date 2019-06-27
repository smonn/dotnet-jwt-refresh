using System.Threading.Tasks;
using JwtRefresh.Models;
using JwtRefresh.Services.Extensions;
using JwtRefresh.Services.Accounts;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace JwtRefresh.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class AccountsController : ControllerBase
    {
        private readonly IAccountService _accountService;

        public AccountsController(IAccountService accountService)
        {
            _accountService = accountService;
        }

        [HttpGet]
        public async Task<ActionResult<GetAccountResponse>> Get()
        {
            var request = new GetAccountRequest
            {
                AccountId = User.GetAccountId(),
            };
            var response = await _accountService.GetAsync(request);
            if (response.Error == null)
            {
                return Ok(response);
            }
            return NotFound(response);
        }

        [HttpPost]
        [AllowAnonymous]
        public async Task<ActionResult<CreateAccountResponse>> Post([FromBody] CreateAccountRequest request)
        {
            var response = await _accountService.RegisterAsync(request);
            if (response.Error == null)
            {
                return Ok(response);
            }
            return BadRequest(response);
        }
    }
}
