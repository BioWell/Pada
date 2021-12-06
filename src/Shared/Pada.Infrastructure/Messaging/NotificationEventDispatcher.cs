using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Humanizer;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using Pada.Abstractions.Messaging;
using Pada.Abstractions.Messaging.Outbox;
using Pada.Abstractions.Persistence;
using Pada.Infrastructure.Messaging.Outbox;
using Pada.Infrastructure.Utils;

namespace Pada.Infrastructure.Messaging
{
    public class NotificationEventDispatcher : INotificationEventDispatcher
    {
        private readonly ILogger<NotificationEventDispatcher> _logger;
        private readonly IServiceProvider _serviceProvider;

        public NotificationEventDispatcher(ILogger<NotificationEventDispatcher> logger,
            IServiceProvider serviceProvider)
        {
            _logger = logger;
            _serviceProvider = serviceProvider;
        }

        public async Task DispatchAsync(params INotificationEvent[] notificationEvents)
        {
            using var scope = _serviceProvider.CreateScope();
            
            foreach (var eventNotification in notificationEvents)
            {
                var data = JsonConvert.SerializeObject(eventNotification,
                    new JsonSerializerSettings
                    {
                        ReferenceLoopHandling = ReferenceLoopHandling.Ignore
                    });

                var outbox = scope.ServiceProvider.GetRequiredService<IOutbox>();
                var outboxOptions = _serviceProvider.GetRequiredService<IOptions<OutboxOptions>>();

                string name = eventNotification.GetType().Name.Underscore();
                if (outboxOptions.Value?.Enabled == false)
                    return;

                var outboxMessage = new OutboxMessage
                {
                    Id = eventNotification.Id,
                    OccurredOn = DateTime.Now,
                    Type = eventNotification.GetType().AssemblyQualifiedName,
                    Name = name,
                    Payload = data,
                    CorrelationId = eventNotification.CorrelationId,
                    ModuleName = ((object) eventNotification).GetModuleName()
                };
                await outbox.SaveAsync(new List<OutboxMessage> {outboxMessage});
                _logger.LogDebug(
                    "DomainEventDispatcher Published domain event notification {DomainEventNotificationName} with payload  {DomainEventNotificationPayload}",
                    name, (string) data);
            }
        }
    }
}