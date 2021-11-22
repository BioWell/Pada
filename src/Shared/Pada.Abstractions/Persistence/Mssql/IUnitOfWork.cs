using System.Threading.Tasks;

namespace Pada.Abstractions.Persistence.Mssql
{
    public interface IUnitOfWork
    {
        Task CommitAsync();
        Task BeginTransactionAsync();
        Task RollbackTransaction();
    }
}