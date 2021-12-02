using System.Collections.Generic;
using Pada.Infrastructure.Domain;
using Pada.Modules.Identity.Domain.Aggregates.Users.Types;

namespace Pada.Modules.Identity.Domain.Aggregates.Users.DomainEvents
{
    public class RolesChangedDomainEvent : DomainEventBase
    {
        public UserId UserId { get; }
        public IEnumerable<Role> Roles { get; }

        public RolesChangedDomainEvent(UserId userId, IEnumerable<Role> roles)
        {
            UserId = userId;
            Roles = roles;
        }
    }
}