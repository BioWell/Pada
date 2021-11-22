using System;

namespace Pada.Abstractions.Persistence
{
    public class OutboxMessage
    {
        public Guid Id { get; set; }
        public Guid CorrelationId { get; set; }
        public string Name { get; set; }
        public string Type { get; set; }
        public string Payload { get; set; }
        public DateTime OccurredOn { get; set; }
        public DateTime? SentAt { get; set; }
        public string ModuleName { get; set; }
    }
}