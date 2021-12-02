using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json.Serialization;
using Pada.Abstractions.Domain;
using Pada.Abstractions.Domain.Types;
using Pada.Abstractions.Exceptions;

namespace Pada.Infrastructure.Domain
{
    public abstract class AggregateRoot<TId> : EntityBase<TId>, IAggregateRoot
    {
        private readonly List<IDomainEvent> _events = new();
        private bool _versionIncremented;
        
        public int Version { get; protected set; }
        
        [JsonIgnore]
        public IEnumerable<IDomainEvent> Events => _events;
        
        protected AggregateRoot()
        {
        }

        protected AggregateRoot(TId id) : base(id)
        {
        }
        
        protected void AddDomainEvent(IDomainEvent @event)
        {
            if (!_events.Any() && !_versionIncremented)
            {
                Version++;
                _versionIncremented = true;
            }

            _events.Add(@event);
        }

        protected void RemoveDomainEvent(IDomainEvent @event)
        {
            _events?.Remove(@event);
        }

        public void ClearEvents()
        {
            _events.Clear();
        }

        protected void CheckRule(IBusinessRule rule)
        {
            if (rule.IsBroken()) throw new BusinessRuleException(rule);
        }

        protected void IncrementVersion()
        {
            if (_versionIncremented) return;

            Version++;
            _versionIncremented = true;
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