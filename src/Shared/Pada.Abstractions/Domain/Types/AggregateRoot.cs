using System;

namespace Pada.Abstractions.Domain.Types
{
    public abstract class AggregateRoot<TId> : EntityBase<TId>, IAggregateRoot
    {
        protected AggregateRoot()
        {
        }

        protected AggregateRoot(TId id) : base(id)
        {
        }
    }
    
    public abstract class AggregateRoot : AggregateRoot<Guid>
    {
        protected AggregateRoot()
        {
        }
        protected AggregateRoot(AggregateId id) : base(id)
        {
        }
    }
}