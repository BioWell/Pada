using System;

namespace Pada.Abstractions.Domain.Types
{
    public abstract class AggregateRoot<TId, TIdentity> : EntityBase<TId, TIdentity>, IAggregateRoot
        where TIdentity : IdentityBase<TId>
    {
        protected AggregateRoot()
        {
        }

        protected AggregateRoot(TIdentity id) : base(id)
        {
        }
    }
    
    public abstract class AggregateRoot : AggregateRoot<Guid, AggregateId>
    {
        protected AggregateRoot()
        {
        }
        protected AggregateRoot(AggregateId id) : base(id)
        {
        }
    }
}