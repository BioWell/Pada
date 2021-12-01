using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Ardalis.GuardClauses;
using MediatR;
using Pada.Modules.Identity.Application.Authentication.Exceptions;
using Pada.Modules.Identity.Application.Users.Contracts;
using Pada.Modules.Identity.Application.Users.Exceptions;

namespace Pada.Modules.Identity.Application.Authentication.Features.RevokeToken
{
    public class RevokeRefreshTokenCommandHandler : IRequestHandler<RevokeRefreshTokenCommand>
    {
        private readonly IUserRepository _userRepository;

        public RevokeRefreshTokenCommandHandler(IUserRepository userRepository)
        {
            _userRepository = userRepository;
        }

        public async Task<Unit> Handle(RevokeRefreshTokenCommand command, CancellationToken cancellationToken = default)
        {
            Guard.Against.Null(command, nameof(RevokeRefreshTokenCommand));
            var user = await _userRepository.FindByRefreshToken(command.RefreshToken);
            if (user == null)
                throw new UserNotFoundException();

            var refreshToken = user.RefreshTokens.SingleOrDefault(x => x.Token == command.RefreshToken);

            if (user.HasValidRefreshToken(command.RefreshToken) == false || refreshToken is null)
                throw new InvalidRefreshTokenException();

            // revoke token and save
            user.RevokeRefreshToken(refreshToken);
            await _userRepository.UpdateAsync(user);

            return Unit.Value;
        }
    }
}