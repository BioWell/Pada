using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading;
using System.Threading.Tasks;
using Ardalis.GuardClauses;
using MediatR;
using Microsoft.Extensions.Logging;
using Microsoft.IdentityModel.JsonWebTokens;
using Pada.Abstractions.Auth;
using Pada.Abstractions.Services.Token;
using Pada.Modules.Identity.Application.Authentication.Dtos;
using Pada.Modules.Identity.Application.Authentication.Exceptions;
using Pada.Modules.Identity.Application.Users.Contracts;
using Pada.Modules.Identity.Application.Users.Exceptions;
using Pada.Modules.Identity.Domain.Aggregates.Users;

namespace Pada.Modules.Identity.Application.Authentication.Features.RefreshToken
{
    public class RefreshTokenCommandHandler : IRequestHandler<RefreshTokenCommand, RefreshTokenResponse>
    {
        private readonly IJwtTokenValidator _tokenValidator;
        private readonly IUserRepository _userRepository;
        private readonly IJwtHandler _jwtHandler;
        private readonly ILogger<RefreshTokenCommandHandler> _logger;
        private readonly ITokenStorageService _tokenStorageService;

        public RefreshTokenCommandHandler(IJwtTokenValidator tokenValidator,
            IUserRepository userRepository, 
            IJwtHandler jwtHandler, 
            ILogger<RefreshTokenCommandHandler> logger,
            ITokenStorageService tokenStorageService)
        {
            _tokenValidator = tokenValidator;
            _userRepository = userRepository;
            _jwtHandler = jwtHandler;
            _logger = logger;
            _tokenStorageService = tokenStorageService;
        }

        public async Task<RefreshTokenResponse> Handle(RefreshTokenCommand command,
            CancellationToken cancellationToken = default)
        {
            Guard.Against.Null(command, nameof(RefreshTokenCommand));

            // invalid token/signing key was passed and we can't extract user claims
            var userPrincipal = _tokenValidator.GetPrincipalFromToken(command.AccessToken);

            if (userPrincipal is null)
                throw new InvalidTokenException();

            var userEmail = userPrincipal.FindFirstValue(JwtRegisteredClaimNames.Email);
            var userName = userPrincipal.FindFirstValue(JwtRegisteredClaimNames.Sub);

            var user = await _userRepository.FindByNameAsync(userName, invalidateCache: true);

            if (user == null)
                throw new UserNotFoundException(userName);

            var refreshToken = user.RefreshTokens.SingleOrDefault(x => x.Token == command.RefreshToken);

            if (user.HasValidRefreshToken(command.RefreshToken) == false || refreshToken is null)
                throw new InvalidRefreshTokenException();

            if (refreshToken.IsRevoked)
            {
                // revoke all descendant tokens in case this token has been compromised
                user.RevokeDescendantRefreshTokens(refreshToken);
                await _userRepository.UpdateAsync(user);
            }

            // replace old refresh token with a new one (rotate token)
            var newRefreshToken = RotateRefreshToken(user, refreshToken);
            user.ChangeRefreshTokens(new List<AppRefreshToken>
            {
                newRefreshToken
            });

            // remove old refresh tokens from user
            // we could also maintain them on the database with changing their revoke date
            user.RemoveOldRefreshTokens();

            _logger.LogInformation("Token refreshed for User with ID: `{Id}`.", user.Id);

            // save changes to db
            await _userRepository.UpdateAsync(user);

            // generate new jwt
            var claims = await _userRepository.GetClaimsAsync(user.UserName);

            var jsonWebToken = _jwtHandler.GenerateJwtToken(user.UserName, user.Email, user.Id.ToString(),
                user.EmailConfirmed, user.FirstName, user.LastName, user.PhoneNumber, claims.UserClaims,
                claims.Roles, claims.PermissionClaims);

            _logger.LogInformation("JsonWebToken generated with this information: {jsonWebToken}", jsonWebToken);

            await _tokenStorageService.SetAsync(command.Id, jsonWebToken);
            
            // await _commandProcessor.PublishMessageAsync(new RefreshTokenIntegrationEvent(user.UserName, user.Id,
            //     jsonWebToken));

            return new RefreshTokenResponse( new RefreshTokenDto(user, jsonWebToken, newRefreshToken.Token));
        }

        private AppRefreshToken RotateRefreshToken(User user, AppRefreshToken refreshToken)
        {
            user.RevokeRefreshToken(refreshToken);
            var newRefreshToken = _jwtHandler.GenerateRefreshToken(user.Id.ToString());
            return newRefreshToken;
        }
    }
}