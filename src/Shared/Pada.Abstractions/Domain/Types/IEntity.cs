using System;

namespace Pada.Abstractions.Domain.Types
{
    public interface IEntity<TId> 
    {
        TId Id { get; }
    }

    public interface IEntity : IEntity<Guid>
    {
    }
}