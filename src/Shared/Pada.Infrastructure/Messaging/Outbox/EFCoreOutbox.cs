using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Pada.Abstractions.Messaging.Outbox;
using Pada.Abstractions.Persistence;
using Pada.Abstractions.Persistence.Mssql;

namespace Pada.Infrastructure.Messaging.Outbox
{
    public class EFCoreOutbox<TContext> : IOutbox where TContext : DbContext, ISqlDbContext
    {
        private readonly ILogger<EFCoreOutbox<TContext>> _logger;
        private readonly TContext _dbContext;

        public EFCoreOutbox(ILogger<EFCoreOutbox<TContext>> logger)
        {
            _logger = logger;
        }

        public bool Enabled { get; }

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

        public Task PublishUnsentAsync()
        {
            throw new System.NotImplementedException();
        }

        public Task<IEnumerable<OutboxMessage>> GetAllOutboxMessages(string moduleName = default)
        {
            throw new System.NotImplementedException();
        }

        public Task CleanProcessedAsync(CancellationToken cancellationToken = default)
        {
            throw new System.NotImplementedException();
        }
    }
}