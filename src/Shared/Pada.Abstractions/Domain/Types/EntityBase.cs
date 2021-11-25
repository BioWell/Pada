using System;
using System.Collections.Generic;

namespace Pada.Abstractions.Domain.Types
{
    public abstract class EntityBase<TId> : IEntity<TId>
    {
        public TId Id { get; protected set; }
        public DateTime Created { get; protected set; } = DateTime.Now;
        public DateTime? Updated { get; protected set; }

        protected EntityBase()
        {
        }

        protected EntityBase(TId id) => Id = id;

        public bool Equals(EntityBase<TId> other)
        {
            if (ReferenceEquals(null, other)) return false;
            if (ReferenceEquals(this, other)) return true;
            return EqualityComparer<TId>.Default.Equals(Id, other.Id) &&
                   Created.Equals(other.Created) &&
                   Nullable.Equals(Updated, other.Updated);
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != this.GetType()) return false;
            return Equals((EntityBase<TId>) obj);
        }

        public override int GetHashCode() => HashCode.Combine(Id, Created, Updated);
        public override string ToString() => $"{GetType().Name}#[Identity={Id}]";
    }

    public abstract class EntityBase : EntityBase<Guid>
    {
        protected EntityBase(Guid id) : base(id)
        {
        }
    }
}