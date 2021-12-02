using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using DbUp;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;
using Pada.Abstractions.Domain;
using Pada.Abstractions.Persistence;
using Pada.Abstractions.Persistence.Mssql;
using Pada.Infrastructure.Domain;

namespace Pada.Infrastructure.Persistence.Mssql
{
    public abstract class SqlDbContextBase : DbContext, ISqlDbContext, IDomainEventContext
    {
        public DbSet<OutboxMessage> OutboxMessages { get; set; }

        private IDbContextTransaction _currentTransaction;

        private readonly IMediator _mediator;
        
        protected void DbUpInitializer(string connection)
        {
            var upgrader =
                DeployChanges.To
                    .SqlDatabase(connection)
                    .WithScriptsEmbeddedInAssembly(Assembly.GetExecutingAssembly())
                    .LogToConsole()
                    .Build();
            var result = upgrader.PerformUpgrade();
        }

        protected SqlDbContextBase(DbContextOptions<SqlDbContextBase> options,
            IMediator mediator) : base(options)
        {
            _mediator = mediator;
            Database.EnsureCreated();
            DbUpInitializer(Database.GetConnectionString());
        }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);
            builder.ApplyConfiguration(new OutboxMessageCfg());
        }

        public async Task BeginTransactionAsync()
        {
            if (_currentTransaction != null)
                return;
            _currentTransaction = await Database.BeginTransactionAsync(IsolationLevel.ReadCommitted);
        }

        public async Task AfterCommitHandler(Func<Task> handler) => await handler();

        public async Task BeforeCommitHandler(Func<Task> handler) => await handler();

        public async Task RollbackTransaction()
        {
            try
            {
                await _currentTransaction?.RollbackAsync();
            }
            finally
            {
                if (_currentTransaction != null)
                {
                    _currentTransaction.Dispose();
                    _currentTransaction = null;
                }
            }
        }

        public async Task CommitTransactionAsync()
        {
            try
            {
                await SaveChangesAsync();
                await (_currentTransaction?.CommitAsync() ?? Task.CompletedTask);
            }
            catch
            {
                await RollbackTransaction();
                throw;
            }
            finally
            {
                if (_currentTransaction != null)
                {
                    _currentTransaction.Dispose();
                    _currentTransaction = null;
                }
            }
        }

        public async Task CommitTransactionWithDispatchEventsAsync()
        {
            var domainEvents = GetDomainEvents().ToList();
            var tasks = domainEvents.Select(async @event =>
            {
                // also will publish our domain event notification internally
                // TODO
                await _mediator.Publish(@event);
            });

            await Task.WhenAll(tasks);
            await CommitTransactionAsync();
        }

        public async Task HandleTransactionAsync(Func<Task> beforeCommitHandler, Func<Task> afterCommitHandler = null,
            IList<IDomainEvent> events = null)
        {
            var strategy = this.Database.CreateExecutionStrategy();
            await strategy.ExecuteAsync(async () =>
            {
                var isInnerTransaction = this.Database.CurrentTransaction is not null;

                if (isInnerTransaction == false)
                    await this.BeginTransactionAsync();

                await this.BeforeCommitHandler(beforeCommitHandler);

                var domainEvents = events == null || events.Any() == false
                    ? this.GetDomainEvents().ToList()
                    : events;
                var tasks = domainEvents.Select(async @event =>
                {
                    // also will publish our domain event notification internally
                    await _mediator.Publish(@event);
                });

                await Task.WhenAll(tasks);

                if (isInnerTransaction == false)
                {
                    await this.CommitTransactionAsync();
                    if (afterCommitHandler is not null)
                        await this.AfterCommitHandler(afterCommitHandler);
                }
            });
        }

        public IEnumerable<IDomainEvent> GetDomainEvents()
        {
            var domainEntities = ChangeTracker
                .Entries<AggregateRoot>()
                .Where(x =>
                    x.Entity.Events != null &&
                    x.Entity.Events.Any())
                .ToList();

            var domainEvents = domainEntities
                .SelectMany(x => x.Entity.Events)
                .ToList();

            domainEntities.ForEach(entity => entity.Entity.Events?.ToList().Clear());

            return domainEvents;
        }
    }
}