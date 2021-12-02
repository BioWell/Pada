using System.Collections.Generic;
using Pada.Infrastructure.Domain;
using Pada.Modules.Identity.Domain.Aggregates.Users.Types;

namespace Pada.Modules.Identity.Domain.Aggregates.Users.DomainEvents
{
    public class PermissionChangedDomainEvent : DomainEventBase
    {
        public UserId UserId { get; }
        public IEnumerable<AppPermission> Permissions { get; }

        public PermissionChangedDomainEvent(UserId userId, IEnumerable<AppPermission> permissions)
        {
            UserId = userId;
            Permissions = permissions;
        }
    }
}