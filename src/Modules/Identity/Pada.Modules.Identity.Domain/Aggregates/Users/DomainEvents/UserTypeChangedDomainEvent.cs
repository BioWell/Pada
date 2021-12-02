using Pada.Infrastructure.Domain;
using Pada.Modules.Identity.Domain.Aggregates.Users.Types;

namespace Pada.Modules.Identity.Domain.Aggregates.Users.DomainEvents
{
    public class UserTypeChangedDomainEvent : DomainEventBase
    {
        public UserId UserId { get; }
        public UserType UserType { get; }

        public UserTypeChangedDomainEvent(UserId userId, UserType userType)
        {
            UserId = userId;
            UserType = userType;
        }
    }
}