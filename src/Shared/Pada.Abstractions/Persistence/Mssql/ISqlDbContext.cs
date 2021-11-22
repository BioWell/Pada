using Microsoft.EntityFrameworkCore;

namespace Pada.Abstractions.Persistence.Mssql
{
    public interface ISqlDbContext
    {
        DbSet<TEntity> Set<TEntity>() where TEntity : class;
    }
}