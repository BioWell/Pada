using System.Collections.Generic;

namespace Pada.Abstractions.Domain
{
    public interface IDomainEventContext
    {
        IEnumerable<IDomainEvent> GetDomainEvents();
    }
}