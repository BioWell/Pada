using Pada.Infrastructure.Domain;

namespace Pada.Modules.Identity.Domain.Aggregates.Users.DomainEvents
{
    public class PersonalInformationChangedDomainEvent : DomainEventBase
    {
        public User User { get; }

        public PersonalInformationChangedDomainEvent(User user)
        {
            User = user;
        }
    }
}