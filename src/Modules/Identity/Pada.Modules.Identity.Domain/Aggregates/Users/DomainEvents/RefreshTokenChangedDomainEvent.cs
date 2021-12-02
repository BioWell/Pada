using System.Collections.Generic;
using Pada.Abstractions.Auth;
using Pada.Infrastructure.Domain;
using Pada.Modules.Identity.Domain.Aggregates.Users.Types;

namespace Pada.Modules.Identity.Domain.Aggregates.Users.DomainEvents
{
    public class RefreshTokenChangedDomainEvent : DomainEventBase
    {
        public UserId UserId { get; }
        public List<AppRefreshToken> RefreshTokens { get; }

        public RefreshTokenChangedDomainEvent(UserId userId, List<AppRefreshToken> refreshTokens)
        {
            UserId = userId;
            RefreshTokens = refreshTokens;
        }
    }
}