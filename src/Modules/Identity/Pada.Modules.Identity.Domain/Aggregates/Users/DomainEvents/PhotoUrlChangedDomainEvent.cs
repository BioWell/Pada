using Pada.Infrastructure.Domain;
using Pada.Modules.Identity.Domain.Aggregates.Users.Types;

namespace Pada.Modules.Identity.Domain.Aggregates.Users.DomainEvents
{
    public class PhotoUrlChangedDomainEvent : DomainEventBase
    {
        public UserId UserId { get; }
        public string PhotoUrl { get; }

        public PhotoUrlChangedDomainEvent(UserId userId, string photoUrl)
        {
            UserId = userId;
            PhotoUrl = photoUrl;
        }
    }
}