using System;

namespace Pada.Abstractions.Messaging
{
    public interface IMessage
    {
        Guid Id { get; set; }
        Guid CorrelationId { get; set; }
        DateTime OccurredOn { get; set; }
    }
}