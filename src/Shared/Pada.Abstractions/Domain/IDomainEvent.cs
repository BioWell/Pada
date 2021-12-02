using System;
using MediatR;

namespace Pada.Abstractions.Domain
{
    public interface IDomainEvent : INotification
    {
        public DateTime OccurredOn { get; }
        public Guid Id { get; set; }
    }
}