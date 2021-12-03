using Pada.Infrastructure.Domain;

namespace Pada.Modules.Identity.Domain.Aggregates.Users.DomainEvents
{
    public class NewUserRegisteredDomainEvent : DomainEventBase
    {
        public User User { get; }

        public NewUserRegisteredDomainEvent(User user)
        {
            User = user;
        }
    }
}