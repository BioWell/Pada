using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Pada.Infrastructure.Web;
using Pada.Modules.Identity.Api.Users.Models.Requests;
using Pada.Modules.Identity.Application.Users.Dtos.UseCaseResponses;
using Pada.Modules.Identity.Application.Users.Features.Activation;
using Pada.Modules.Identity.Application.Users.Features.GetUser;
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
            if (request.Id is null) request.Id = Guid.NewGuid();
            if (request.UserName is null) request.UserName = request.Email;

            var command = Mapper.Map<RegisterNewUserCommand>(request);
            await Mediator.Send(command);
            var name = nameof(GetUserByIdAsync);
            return CreatedAtRoute(name, new {id = command.Id}, command);
        }

        // GET api/v1/identity/users/id/{userId}
        [HttpGet("id/{id}", Name = nameof(GetUserByIdAsync))]
        [AllowAnonymous]
        public async Task<ActionResult<UserDto>> GetUserByIdAsync([FromRoute] Guid id)
        {
            var result = await Mediator.Send(new GetUserByIdQuery(id));

            return Ok(result);
        }
        
        // GET api/v1/identity/users/email/{email}
        [HttpGet("email/{email}", Name = nameof(GetUserByEmailAsync))]
        [AllowAnonymous]
        public async Task<ActionResult<UserDto>> GetUserByEmailAsync([FromRoute] string email)
        {
            var result = await Mediator.Send(new GetUserByEmailQuery(email));

            return Ok(result);
        }
        
        // GET api/v1/identity/users/UserName/{UserName}
        [HttpGet("UserName/{UserName}", Name = nameof(GetUserByNameAsync))]
        [AllowAnonymous]
        public async Task<ActionResult<UserDto>> GetUserByNameAsync([FromRoute] string UserName)
        {
            var result = await Mediator.Send(new GetUserByUserNameQuery(UserName));

            return Ok(result);
        }

        // Put api/v1/identity/users/active-user
        [HttpPut("active-user")]
        [AllowAnonymous]
        public async Task<ActionResult> ActivateUsersync(ActivateUserCommand request)
        {
            var result = await Mediator.Send(new ActivateUserCommand(request.UserId));
            return Ok(result);
        }
        
        // Put api/v1/identity/users/deactive-user
        [HttpPut("deactive-user")]
        [AllowAnonymous]
        public async Task<ActionResult> DeActivateUsersync(ActivateUserCommand request)
        {
            var result = await Mediator.Send(new DeActivateUserCommand(request.UserId));
            return Ok(result);
        }
    }
}