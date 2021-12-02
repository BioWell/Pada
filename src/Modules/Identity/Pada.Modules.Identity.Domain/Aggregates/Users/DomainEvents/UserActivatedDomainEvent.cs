using Pada.Infrastructure.Domain;
using Pada.Modules.Identity.Domain.Aggregates.Users.Types;

namespace Pada.Modules.Identity.Domain.Aggregates.Users.DomainEvents
{
    public class UserActivatedDomainEvent : DomainEventBase
    {
        public UserId UserId { get; }

        public UserActivatedDomainEvent(UserId userId)
        {
            UserId = userId;
        }
    }
}