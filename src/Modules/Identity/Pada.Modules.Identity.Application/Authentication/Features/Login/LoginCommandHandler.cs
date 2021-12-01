using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Ardalis.GuardClauses;
using MediatR;
using Microsoft.Extensions.Logging;
using Pada.Abstractions.Auth;
using Pada.Abstractions.Services.Token;
using Pada.Infrastructure.Auth;
using Pada.Modules.Identity.Application.Authentication.Dtos;
using Pada.Modules.Identity.Application.Authentication.Exceptions;
using Pada.Modules.Identity.Application.Users.Contracts;
using Pada.Modules.Identity.Application.Users.Exceptions;

namespace Pada.Modules.Identity.Application.Authentication.Features.Login
{
    public class LoginCommandHandler : IRequestHandler<LoginCommand, LoginResponse>
    {
        private readonly IUserRepository _userRepository;
        private readonly ILogger<LoginCommandHandler> _logger;
        private readonly IJwtHandler _jwtHandler;
        private readonly JwtOptions _jwtOptions;
        private readonly ITokenStorageService _tokenStorageService;

        public LoginCommandHandler(IUserRepository userRepository,
            ILogger<LoginCommandHandler> logger,
            IJwtHandler jwtHandler,
            JwtOptions jwtOptions, ITokenStorageService tokenStorageService)
        {
            _userRepository = userRepository;
            _logger = logger;
            _jwtHandler = jwtHandler;
            _jwtOptions = jwtOptions;
            _tokenStorageService = tokenStorageService;
        }

        public async Task<LoginResponse> Handle(LoginCommand command,
            CancellationToken cancellationToken = default)
        {
            Guard.Against.Null(command, nameof(LoginCommand));

            (var user, var isLockedResult) = await _userRepository.IsUserLockedAsync(command.UserNameOrEmail);

            if (user is null)
                throw new UserNotFoundException(user.Id);

            if (isLockedResult == true)
                throw new UserLockedException(user.Id.ToString());

            var passwordIsValidResult = await _userRepository.CheckPassword(user.UserName, command.Password);

            if (passwordIsValidResult.IsSuccess == false || passwordIsValidResult.IsPasswordValid == false)
                throw new PasswordIsInvalidException();

            if (!user.IsActive)
                throw new UserInactiveException(command.UserNameOrEmail);

            if (user.EmailConfirmed == false)
                throw new EmailNotConfirmedException(user.Email);

            // authentication successful so generate jwt and refresh tokens
            var allClaims = await _userRepository.GetClaimsAsync(user.UserName);

            var jsonWebToken = _jwtHandler.GenerateJwtToken(user.UserName, user.Email, user.Id.ToString(),
                user.EmailConfirmed, user.FirstName, user.LastName, user.PhoneNumber,
                allClaims.UserClaims, allClaims.Roles, allClaims.PermissionClaims);

            var newRefreshToken = _jwtHandler.GenerateRefreshToken(user.Id.ToString());

            user.ChangeRefreshTokens(new List<AppRefreshToken>
            {
                newRefreshToken
            });

            // remove old refresh tokens from user
            user.RemoveOldRefreshTokens(_jwtOptions.RefreshTokenTTL);

            // save changes to db
            await _userRepository.UpdateAsync(user);

            _logger.LogInformation("JsonWebToken generated with this information: {jsonWebToken}", jsonWebToken);

            await _tokenStorageService.SetAsync(command.Id, jsonWebToken);
            _logger.LogInformation("User with ID: {Id} has been authenticated.", user.Id);

            // await _commandProcessor.PublishMessageAsync(new LoggedInIntegrationEvent(user.UserName, user.Id,
            //     jsonWebToken));

            // we can don't return value from command and get token from a short term session in our request with `TokenStorageService`
            return new LoginResponse(new LoginDto(user, jsonWebToken, newRefreshToken.Token));
        }
    }
}