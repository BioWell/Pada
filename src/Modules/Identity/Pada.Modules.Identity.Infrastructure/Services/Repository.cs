using System;
using Pada.Abstractions.Domain.Types;
using Pada.Infrastructure.Persistence.Mssql;
using Pada.Modules.Identity.Infrastructure.Persistence;

namespace Pada.Modules.Identity.Infrastructure.Services
{
    public class Repository<TEntity, TId, TIdentity> : RepositoryBase<AppIdentityDbContext, TEntity, TId, TIdentity>
        where TEntity : AggregateRoot<TId, TIdentity>
        where TIdentity : IdentityBase<TId>
    {
        public Repository(AppIdentityDbContext dbContext) : base(dbContext)
        {
        }
    }
    public class Repository<TEntity> : Repository<TEntity, Guid, AggregateId> where TEntity : AggregateRoot
    {
        public Repository(AppIdentityDbContext dbContext) : base(dbContext)
        {
        }
    }
}