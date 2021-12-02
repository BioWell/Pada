using System;
using System.Data;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using EntityFramework.Exceptions.SqlServer;
using MediatR;
using Microsoft.EntityFrameworkCore.Storage;
using Pada.Abstractions.Domain;
using Pada.Abstractions.Persistence;
using Pada.Modules.Identity.Application;
using Pada.Modules.Identity.Infrastructure.Aggregates.Roles;
using Pada.Modules.Identity.Infrastructure.Aggregates.Users;
using Pada.Modules.Identity.Infrastructure.Persistence.Configurations;

namespace Pada.Modules.Identity.Infrastructure.Persistence
{
    public sealed class AppIdentityDbContext : IdentityDbContext<
            AppUser, AppRole, string,
            IdentityUserClaim<string>,
            AppUserRole, IdentityUserLogin<string>,
            IdentityRoleClaim<string>, IdentityUserToken<string>>,
        IAppIdentityDbContext
    {
        private IDbContextTransaction _currentTransaction;

        public DbSet<OutboxMessage> OutboxMessages { get; set; }
        private readonly IMediator _mediator;

        public AppIdentityDbContext(DbContextOptions<AppIdentityDbContext> options,
            IMediator mediator) : base(options)
        {
            _mediator = mediator;
        }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder) =>
            optionsBuilder.UseExceptionProcessor();

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);

            builder.ApplyConfiguration(new AppRoleCfg());
            builder.ApplyConfiguration(new AppUserCfg());

            builder.Entity<IdentityUserClaim<string>>().Property(x => x.UserId).HasMaxLength(128);
            builder.Entity<IdentityUserLogin<string>>().Property(x => x.UserId).HasMaxLength(128);
            builder.Entity<IdentityUserLogin<string>>().Property(x => x.LoginProvider).HasMaxLength(128);
            builder.Entity<IdentityUserLogin<string>>().Property(x => x.ProviderKey).HasMaxLength(128);
            builder.Entity<AppUserRole>().Property(x => x.UserId).HasMaxLength(128);
            builder.Entity<AppUserRole>().Property(x => x.RoleId).HasColumnName("RoleCode").HasMaxLength(50);
            builder.Entity<IdentityRoleClaim<string>>().Property(x => x.RoleId).HasColumnName("RoleCode")
                .HasMaxLength(50);
            builder.Entity<IdentityUserToken<string>>().Property(x => x.UserId).HasMaxLength(128);

            MapsTables(builder);
        }

        private static void MapsTables(ModelBuilder builder)
        {
            builder.Entity<AppUser>(b => { b.ToTable("User"); }).HasDefaultSchema("identities");
            builder.Entity<IdentityUserClaim<string>>(b => { b.ToTable("UserClaim"); }).HasDefaultSchema("identities");
            builder.Entity<IdentityUserLogin<string>>(b => { b.ToTable("UserLogin"); }).HasDefaultSchema("identities");
            builder.Entity<IdentityUserToken<string>>(b => { b.ToTable("UserToken"); }).HasDefaultSchema("identities");
            builder.Entity<AppRole>(b => { b.ToTable("Role"); }).HasDefaultSchema("identities");
            builder.Entity<AppUserRole>(b => { b.ToTable("UserRoles"); }).HasDefaultSchema("identities");
            builder.Entity<IdentityRoleClaim<string>>(b => { b.ToTable("RoleClaim"); }).HasDefaultSchema("identities");
        }

        public async Task CommitTransactionWithDispatchEventsAsync()
        {
            var domainEvents = GetDomainEvents().ToList();
            var tasks = domainEvents.Select(async @event =>
            {
                // also will publish our domain event notification internally
                await _mediator.Publish(@event);
            });

            await Task.WhenAll(tasks);
            await CommitTransactionAsync();
        }

        public async Task AfterCommitHandler(Func<Task> handler) => await handler();

        public async Task BeforeCommitHandler(Func<Task> handler) => await handler();
        
        public async Task BeginTransactionAsync()
        {
            if (_currentTransaction != null)
            {
                return;
            }

            _currentTransaction = await Database.BeginTransactionAsync(IsolationLevel.ReadCommitted);
        }

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
                .Entries<IAggregateRoot>()
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