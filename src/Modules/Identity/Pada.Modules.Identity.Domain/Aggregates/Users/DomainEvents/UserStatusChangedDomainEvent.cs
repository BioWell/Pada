using Pada.Infrastructure.Domain;
using Pada.Modules.Identity.Domain.Aggregates.Users.Types;

namespace Pada.Modules.Identity.Domain.Aggregates.Users.DomainEvents
{
    public class UserStatusChangedDomainEvent : DomainEventBase
    {
        public UserId UserId { get; }
        public string Status { get; }

        public UserStatusChangedDomainEvent(UserId userId, string status)
        {
            UserId = userId;
            Status = status;
        }
    }
}