using Pada.Infrastructure.Domain;
using Pada.Modules.Identity.Domain.Aggregates.Users.Types;

namespace Pada.Modules.Identity.Domain.Aggregates.Users.DomainEvents
{
    public class UserMarkedAsUserDomainEvent : DomainEventBase
    {
        public UserId UserId { get; }

        public UserMarkedAsUserDomainEvent(UserId userId)
        {
            UserId = userId;
        }
    }
}