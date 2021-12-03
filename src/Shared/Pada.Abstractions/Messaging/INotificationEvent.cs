using System;
using MediatR;

namespace Pada.Abstractions.Messaging
{
    public class INotificationEvent: INotification
    {
        public Guid Id { get; set; }
        public Guid CorrelationId { get; set; }
    }
}