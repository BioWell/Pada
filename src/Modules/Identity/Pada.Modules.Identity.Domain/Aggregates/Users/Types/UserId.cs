using System;
using Pada.Abstractions.Domain.Types;

namespace Pada.Modules.Identity.Domain.Aggregates.Users.Types
{
    public class UserId : EntityBase
    {
        public UserId(Guid value) : base(value)
        {
        }
        
        public UserId(Guid id, DateTime created, DateTime updated) : base(id)
        {
            Created = created;
            Updated = updated;
        }

        public static implicit operator UserId(Guid id) => new(id);

        public static implicit operator Guid(UserId userId) => userId.Id;
    }
}