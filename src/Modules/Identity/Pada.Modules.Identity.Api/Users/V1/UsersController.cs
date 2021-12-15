using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Pada.Infrastructure.Web;
using Pada.Modules.Identity.Api.Users.Models.Requests;
using Pada.Modules.Identity.Application.Users.Dtos;
using Pada.Modules.Identity.Application.Users.Features.Activation;
using Pada.Modules.Identity.Application.Users.Features.GetUser;
using Pada.Modules.Identity.Application.Users.Features.Lock;
using Pada.Modules.Identity.Application.Users.Features.PersonalInformation;
using Pada.Modules.Identity.Application.Users.Features.RegisterNewUser;
using Pada.Modules.Identity.Domain;

namespace Pada.Modules.Identity.Api.Users.V1
{
    [Route(BaseApiPath + "/" + IdentityModule.ModulePath + "/[controller]")]
    [ApiVersion("1.0")]
    public class UsersController : BaseController
    {
        // POST api/v1/identity/users/Register
        [HttpPost("Register")]
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

        // POST api/v1/identity/users/RegisterByPhone
        [HttpPost("RegisterByPhone")]
        [AllowAnonymous]
        public async Task<ActionResult> RegisterByPhoneAsync(RegisterNewUserByPhoneRequest request)
        {
            var command = Mapper.Map<RegisterNewUserByPhoneRequest>(request);
            await Mediator.Send(command);
            var name = nameof(FindByPhoneAsync);
            return CreatedAtRoute(name, new {id = command.Phone}, command);
        }
        
        // GET api/v1/identity/users/roles
        // [HttpGet("id/{id}", Name = nameof(GetUserByIdAsync))]
        // [AllowAnonymous]
        // public async Task<ActionResult<UserDto>> GetUserByIdAsync([FromRoute] Guid id)
        // {
        //     var result = await Mediator.Send(new GetUserByIdQuery(id));
        //
        //     return Ok(result);
        // }

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

        // GET api/v1/identity/users/Phone/{phone}
        [HttpGet("Phone/{phone}", Name = nameof(FindByPhoneAsync))]
        [AllowAnonymous]
        public async Task<ActionResult<UserDto>> FindByPhoneAsync([FromRoute] string phone)
        {
            var result = await Mediator.Send(new GetUserByPhoneQuery(phone));

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

        // POST api/v1/identity/users/{userId}/lock-user
        [HttpPost("{userId}/lock-user")]
        [AllowAnonymous]
        public async Task<IActionResult> LockUserasync([FromRoute] string userId)
        {
            var result = await Mediator.Send(new LockUserCommand(userId));
            return Ok(result);
        }

        // POST api/v1/identity/users/{userId}/send-verification-email
        [HttpPost("{userId}/send-verification-email")]
        [Authorize(SecurityConstants.Permission.Security.VerifyEmail)]
        public async Task<ActionResult> SendVerificationEmailAsync([FromRoute] string userId)
        {
            await Mediator.Send(new SendVerificationEmailCommand(userId));

            return NoContent();
        }
        
        // POST api/v1/identity/users/{userId}/verify-email
        [HttpGet("{userId}/verify-email")]
        [AllowAnonymous]
        public async Task<ActionResult> VerifyEmailAsync([FromRoute] string userId, [FromQuery] string code)
        {
            await Mediator.Send(new VerifyEmailCommand(userId, code));

            return NoContent();
        }
        
        // PUT api/v1/identity/users/update-personal-info
        [HttpPut("update-personal-info")]
        // [AllowAnonymous]
        [Authorize(SecurityConstants.Permission.Users.Edit)]
        public async Task<ActionResult> UpdatePersonalInformation([FromBody] ChangePersonalInfoRequest userInfoRequest)
        {
            var command = Mapper.Map<ChangePersonalInformationCommand>(userInfoRequest);
            await Mediator.Send(command);
            return NoContent();
        }
    }
}