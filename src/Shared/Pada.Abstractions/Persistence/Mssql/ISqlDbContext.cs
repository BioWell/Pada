using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Pada.Abstractions.Domain;

namespace Pada.Abstractions.Persistence.Mssql
{
    public interface ISqlDbContext : IDbFacadeResolver, IDomainEventContext
    {
        DbSet<OutboxMessage> OutboxMessages { get; set; }
        DbSet<TEntity> Set<TEntity>() where TEntity : class;

        Task BeginTransactionAsync();
        Task RollbackTransaction();
        Task AfterCommitHandler(Func<Task> handler);
        Task BeforeCommitHandler(Func<Task> handler);
        Task CommitTransactionAsync();
        Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);
        Task CommitTransactionWithDispatchEventsAsync();
        Task HandleTransactionAsync(Func<Task> beforeCommitHandler, Func<Task> afterCommitHandler = null,
            IList<IDomainEvent> events = null);
    }
}