using Pada.Infrastructure.Domain;
using Pada.Modules.Identity.Domain.Aggregates.Users.Types;

namespace Pada.Modules.Identity.Domain.Aggregates.Users.DomainEvents
{
    public class UserDeactivateDomainEvent : DomainEventBase
    {
        public UserId UserId { get; }

        public UserDeactivateDomainEvent(UserId userId)
        {
            UserId = userId;
        }
    }
}