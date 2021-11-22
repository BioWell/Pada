using Microsoft.EntityFrameworkCore.Infrastructure;

namespace Pada.Abstractions.Persistence.Mssql
{
    public interface IDbFacadeResolver
    {
        DatabaseFacade Database { get; }
    }
}