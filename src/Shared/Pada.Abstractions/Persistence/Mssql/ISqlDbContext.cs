using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;

namespace Pada.Abstractions.Persistence.Mssql
{
    public interface ISqlDbContext :IDbFacadeResolver//, IDomainEventContext
    {
        DbSet<OutboxMessage> OutboxMessages { get; set; }
        DbSet<TEntity> Set<TEntity>() where TEntity : class;

        Task BeginTransactionAsync();
        Task RollbackTransaction();
    }
}