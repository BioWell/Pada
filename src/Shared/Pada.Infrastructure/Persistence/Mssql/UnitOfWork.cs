using System.Threading.Tasks;
using Pada.Abstractions.Persistence.Mssql;

namespace Pada.Infrastructure.Persistence.Mssql
{
    public class UnitOfWork : IUnitOfWork
    {
        private readonly ISqlDbContext _dbContext;

        public UnitOfWork(ISqlDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task CommitAsync()
        {
            await Task.CompletedTask;
        }

        public async Task BeginTransactionAsync()
        {
            await _dbContext.BeginTransactionAsync();
        }

        public async Task RollbackTransaction()
        {
            await _dbContext.RollbackTransaction();
        }
    }
}