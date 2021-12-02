using Pada.Infrastructure.Domain;

namespace Pada.Modules.Identity.Domain.Aggregates.Users.DomainEvents
{
    public class NewUserRegisteredDomainEvent : DomainEventBase
    {
        public NewUserRegisteredDomainEvent(User user)
        {
            User = user;
        }

        public User User { get; }
    }
}