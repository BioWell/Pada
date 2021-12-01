using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Pada.Infrastructure.Web;
using Pada.Modules.Identity.Api.Authentications.Models.Requests;
using Pada.Modules.Identity.Application.Authentication.Dtos;
using Pada.Modules.Identity.Application.Authentication.Features.Login;
using Pada.Modules.Identity.Application.Authentication.Features.RefreshToken;

namespace Pada.Modules.Identity.Api.Authentications.v1
{
    [Route(BaseApiPath + "/" + IdentityModule.ModulePath + "/[controller]")]
    [ApiVersion("1.0")]
    public class AuthController : BaseController
    {
        // POST api/v1/identity/auth/login
        [HttpPost("login")]
        [AllowAnonymous]
        public async Task<ActionResult<LoginDto>> LoginAsync([FromBody] LoginRequest loginRequest)
        {
            var command = Mapper.Map<LoginCommand>(loginRequest);
            var loginCommandResponse = await Mediator.Send(command);

            return Ok(loginCommandResponse);
        }
        
        // POST api/v1/auth/refresh-token
        [HttpPost("refresh-token")]
        [AllowAnonymous]
        public async Task<ActionResult<RefreshTokenResponse>> Login([FromBody] RefreshTokenRequest refreshTokenRequest)
        {
            var command = Mapper.Map<RefreshTokenCommand>(refreshTokenRequest);
            var refreshCommandResponse = await Mediator.Send(command);
        
            return Ok(refreshCommandResponse);
        }
    }
}