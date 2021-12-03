using Pada.Infrastructure.Domain;
using Pada.Modules.Identity.Domain.Aggregates.Users.Types;

namespace Pada.Modules.Identity.Domain.Aggregates.Users.DomainEvents
{
    public class UserMarkedAsAdminDomainEvent : DomainEventBase
    {
        public UserId UserId { get; }

        public UserMarkedAsAdminDomainEvent(UserId userId)
        {
            UserId = userId;
        }
    }
}