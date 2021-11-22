using System.Data;
using System.Reflection;
using System.Threading.Tasks;
using DbUp;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;
using Pada.Abstractions.Persistence;
using Pada.Abstractions.Persistence.Mssql;

namespace Pada.Infrastructure.Persistence.Mssql
{
    public abstract class SqlDbContextBase : DbContext, ISqlDbContext//, IDomainEventContext
    {
        public DbSet<OutboxMessage> OutboxMessages { get; set; }
        
        private IDbContextTransaction _currentTransaction;
        
        protected SqlDbContextBase(DbContextOptions<SqlDbContextBase> options) : base(options)
        {
            Database.EnsureCreated();
            DbUpInitializer(Database.GetConnectionString());
        }
        
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
    }
}