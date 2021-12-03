using Pada.Infrastructure.Domain;
using Pada.Modules.Identity.Domain.Aggregates.Users.Types;

namespace Pada.Modules.Identity.Domain.Aggregates.Users.DomainEvents
{
    public class UserNameChangedDomainEvent : DomainEventBase
    {
        public UserId UserId { get; }
        public string NewUserName { get; }

        public UserNameChangedDomainEvent(UserId userId, string newUserName)
        {
            UserId = userId;
            NewUserName = newUserName;
        }
    }
}