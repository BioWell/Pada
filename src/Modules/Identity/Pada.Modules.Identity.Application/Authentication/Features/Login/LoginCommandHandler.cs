using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Ardalis.GuardClauses;
using MediatR;
using Microsoft.Extensions.Logging;
using Pada.Modules.Identity.Application.Authentication.Dtos;
using Pada.Modules.Identity.Application.Users.Contracts;
using Pada.Modules.Identity.Application.Users.Exceptions;

namespace Pada.Modules.Identity.Application.Authentication.Features.Login
{
    public class LoginCommandHandler : IRequestHandler<LoginCommand, LoginCommandResponse>
    {
        private readonly IUserRepository _userRepository;
        private readonly ILogger<LoginCommandHandler> _logger;

        public LoginCommandHandler(IUserRepository userRepository,
            ILogger<LoginCommandHandler> logger)
        {
            _userRepository = userRepository;
            _logger = logger;
        }

        public async Task<LoginCommandResponse> Handle(LoginCommand command,
            CancellationToken cancellationToken = default)
        {
            Guard.Against.Null(command, nameof(LoginCommand));

            // var user = await _userRepository.FindByNameOrEmailAsync(command.UserNameOrEmail);
            //
            // if (user == null)
            // {
            //     throw new UserNotFoundException(command.UserNameOrEmail);
            // }
            //
            // var isLockedResult = await _userRepository.IsUserLockedAsync(user.Id.ToString());
            // if (isLockedResult.IsSuccess == false)
            //     isLockedResult.ToAppException();
            //
            // if (isLockedResult.IsLocked)
            //     throw new UserLockedException(user.Id.ToString());
            //
            // var passwordIsValidResult = await _userRepository.CheckPassword(user.UserName, command.Password);
            //
            // if (passwordIsValidResult.IsSuccess == false)
            // {
            //     throw passwordIsValidResult.ToAppException();
            // }
            //
            // if (passwordIsValidResult.IsPasswordValid == false)
            // {
            //     throw new PasswordIsInvalidException();
            // }
            //
            // if (!user.IsActive)
            // {
            //     throw new UserInactiveException(command.UserNameOrEmail);
            // }
            //
            // if (user.EmailConfirmed == false)
            // {
            //     throw new EmailNotConfirmedException(user.Email);
            // }
            //
            // // authentication successful so generate jwt and refresh tokens
            // var allClaims = await _userRepository.GetClaimsAsync(user.UserName);
            //
            // var jsonWebToken = _jwtHandler.GenerateJwtToken(user.UserName, user.Email, user.Id.Id.ToString(),
            //     user.EmailConfirmed, user.FirstName, user.LastName, user.PhoneNumber,
            //     allClaims.UserClaims, allClaims.Roles, allClaims.PermissionClaims);
            //
            // var newRefreshToken = _jwtHandler.GenerateRefreshToken(user.Id.Id.ToString());
            //
            // user.ChangeRefreshTokens(new List<BuildingBlocks.Authentication.Jwt.RefreshToken>
            // {
            //     newRefreshToken
            // });
            //
            // // remove old refresh tokens from user
            // user.RemoveOldRefreshTokens(_jwtOptions.RefreshTokenTTL);
            //
            // // save changes to db
            // await _userRepository.UpdateAsync(user);
            //
            // _logger.LogInformation("JsonWebToken generated with this information: {jsonWebToken}", jsonWebToken);
            //
            // await _tokenStorageService.SetAsync(command.Id, jsonWebToken);
            // _logger.LogInformation("User with ID: {Id} has been authenticated.", user.Id);
            //
            // await _commandProcessor.PublishMessageAsync(new LoggedInIntegrationEvent(user.UserName, user.Id,
            //     jsonWebToken));
            //
            // // we can don't return value from command and get token from a short term session in our request with `TokenStorageService`
            // return new LoginCommandResponse(user, jsonWebToken, newRefreshToken.Token);
        }
    }
}