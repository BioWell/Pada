using Pada.Abstractions.Auth;
using Pada.Infrastructure.Domain;
using Pada.Modules.Identity.Domain.Aggregates.Users.Types;

namespace Pada.Modules.Identity.Domain.Aggregates.Users.DomainEvents
{
    public class RefreshTokenRevokedDomainEvent : DomainEventBase
    {
        public UserId UserId { get; }
        public AppRefreshToken RefreshToken { get; }

        public RefreshTokenRevokedDomainEvent(UserId userId, AppRefreshToken refreshToken)
        {
            UserId = userId;
            RefreshToken = refreshToken;
        }
    }
}