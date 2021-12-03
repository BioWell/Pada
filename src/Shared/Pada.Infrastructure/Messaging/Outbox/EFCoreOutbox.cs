using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Pada.Abstractions.Messaging;
using Pada.Abstractions.Messaging.Outbox;
using Pada.Abstractions.Messaging.Serialization;
using Pada.Abstractions.Persistence;
using Pada.Abstractions.Persistence.Mssql;
using Pada.Infrastructure.Utils;

namespace Pada.Infrastructure.Messaging.Outbox
{
    public class EFCoreOutbox<TContext> : IOutbox where TContext : DbContext, ISqlDbContext
    {
        private readonly ILogger<EFCoreOutbox<TContext>> _logger;
        private readonly TContext _dbContext;
        private readonly IMessageSerializer _messageSerializer;
        private readonly IMediator _mediator;
        
        public bool Enabled { get; }
        
        public EFCoreOutbox(IOptions<OutboxOptions> options,
            ILogger<EFCoreOutbox<TContext>> logger,
            TContext dbContext, 
            IMessageSerializer messageSerializer,
            IMediator mediator)
        {
            _logger = logger;
            _dbContext = dbContext;
            _messageSerializer = messageSerializer;
            _mediator = mediator;
            Enabled = options.Value.Enabled;
        }

        public async Task SaveAsync(IList<OutboxMessage> outboxMessages, CancellationToken cancellationToken = default)
        {
            if (!Enabled)
            {
                _logger.LogWarning("Outbox is disabled, outgoing messages won't be saved.");
                return;
            }

            if (outboxMessages is null || !outboxMessages.Any())
            {
                _logger.LogWarning("No messages have been provided to be saved to the outbox.");
                return;
            }

            await _dbContext.OutboxMessages.AddRangeAsync(outboxMessages, cancellationToken);
            await _dbContext.SaveChangesAsync(cancellationToken);

            _logger.LogInformation("Saved {Count} messages to the outbox.", outboxMessages.Count);
            _logger.LogInformation(
                $"Saved outbox messages id are " +
                $"'{string.Join(',', outboxMessages.Select(x => x.Id.ToString()))}' " +
                $"in '{string.Join(',', outboxMessages.Select(x => x.ModuleName))}' modules");
        }

        public async Task PublishUnsentAsync()
        {
            var unsentMessages = await _dbContext.OutboxMessages
                .AsQueryable()
                .Where(x => x.SentAt == null)
                .ToListAsync();

            if (!unsentMessages.Any())
            {
                _logger.LogTrace("No unsent messages found in outbox.");
                return;
            }

            _logger.LogInformation("Found {Count} unsent messages in outbox, sending...",
                unsentMessages.Count);

            foreach (var outboxMessage in unsentMessages)
            {
                var type = Type.GetType(outboxMessage.Type);

                var data = _messageSerializer.Deserialize(outboxMessage.Payload, type);
                if (data is null)
                {
                    _logger.LogError("Invalid message type: {Name}.", type?.Name);
                    continue;
                }

                if (data is INotificationEvent notification)
                {
                    // domain event notification
                    await _mediator.Publish(@notification);
                    _logger.LogInformation(
                        "Published a notification: '{Name}' with ID: '{Id} (outbox)'.",
                        outboxMessage.Name, notification.Id);
                }
                
                if (data is IMessage message)
                {
                    // integration event
                    await _mediator.Publish(message);
                    _logger.LogInformation(
                        "Published a message: '{Name}' with ID: '{Id} (outbox)'.",
                        outboxMessage.Name, message.Id);
                }

                outboxMessage.SentAt = DateTime.Now;
            }

            await _dbContext.SaveChangesAsync().ConfigureAwait(false);
        }

        public async Task<IEnumerable<OutboxMessage>> GetAllOutboxMessages(string moduleName = default)
        {
            var messages = await _dbContext.OutboxMessages
                .Where(x => moduleName.EmumIsNullOrEmpty() || x.ModuleName == moduleName)
                .AsQueryable()
                .ToListAsync();

            return messages;
        }

        public async Task CleanProcessedAsync(CancellationToken cancellationToken = default)
        {
            var filter = _dbContext.OutboxMessages.Where(e => e.SentAt != null);
            _dbContext.OutboxMessages.RemoveRange(filter);
            await _dbContext.SaveChangesAsync(cancellationToken);
        }
    }
}