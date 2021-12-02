using System.Collections.Generic;

namespace Pada.Abstractions.Domain
{
    public interface IAggregateRoot
    {
        IEnumerable<IDomainEvent> Events { get; }
    }
}