using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Pada.Infrastructure.Web;
using Pada.Modules.Identity.Api.Users.Models.Requests;
using Pada.Modules.Identity.Application.Users.Dtos;
using Pada.Modules.Identity.Application.Users.Features.GetUserById;
using Pada.Modules.Identity.Application.Users.Features.RegisterNewUser;

namespace Pada.Modules.Identity.Api.Users.V1
{
    [Route(BaseApiPath + "/" + IdentityModule.ModulePath + "/[controller]")]
    [ApiVersion("1.0")]
    public class UsersController : BaseController
    {
        private readonly IHttpContextAccessor _httpContextAccessor;

        public UsersController(IHttpContextAccessor httpContextAccessor)
        {
            _httpContextAccessor = httpContextAccessor;
        }
        
        // POST api/v1/identity/users
        [HttpPost]
        [AllowAnonymous]
        public async Task<ActionResult> RegisterAsync(RegisterNewUserRequest request)
        {
            var command = Mapper.Map<RegisterNewUserCommand>(request);
            var brand = await Mediator.Send(command);

            var name = nameof(GetUserByIdAsync);
            return CreatedAtAction(name, new { id = command.Id }, command);
        }
        
        // GET api/v1/identity/users/id/{userId}
        [HttpGet("id/{id}")]
        [AllowAnonymous]
        public async Task<ActionResult<UserDto>> GetUserByIdAsync([FromRoute] Guid id)
        {
            var result = await Mediator.Send(new GetUserByIdQuery(id));
  
            return Ok(result);
        }
    }
}