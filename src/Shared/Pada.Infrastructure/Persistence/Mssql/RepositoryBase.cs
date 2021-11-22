using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Pada.Abstractions.Domain.Types;
using Pada.Abstractions.Persistence;
using Pada.Abstractions.Persistence.Mssql;
using Pada.Abstractions.Persistence.Specification;
using Pada.Infrastructure.Exceptions;

namespace Pada.Infrastructure.Persistence.Mssql
{
    public class RepositoryBase<TDbContext, TEntity, TId, TIdentity> : 
        IRepository<TEntity, TId, TIdentity>
        where TEntity : AggregateRoot<TId, TIdentity>
        where TDbContext : DbContext, ISqlDbContext
        where TIdentity : IdentityBase<TId>
    {
        protected RepositoryBase(TDbContext dbContext)
        {
            DbContext = dbContext ?? throw CoreException.NullArgument(nameof(dbContext));
        }

        protected TDbContext DbContext { get; }

        public TEntity Add(TEntity entity)
        {
            DbContext.Set<TEntity>().Add(entity);

            return entity;
        }

        public Task AddRangeAsync(IEnumerable<TEntity> entities)
        {
           return DbContext.Set<TEntity>().AddRangeAsync(entities);
        }

        public virtual IAsyncEnumerable<TEntity> FindAllAsync(ISpecification<TEntity> specification = null)
        {
            var queryable = DbContext.Set<TEntity>().AsQueryable();
            if (specification == null)
            {
                return queryable
                    .AsNoTracking()
                    .AsAsyncEnumerable();
            }

            var queryableWithInclude = specification.Includes
                .Aggregate(queryable, (current, include) => current.Include(include));

            return queryableWithInclude
                .Where(specification.Criteria)
                .AsNoTracking()
                .AsAsyncEnumerable();
        }

        public void Remove(TEntity entity)
        {
            DbContext.Set<TEntity>().Remove(entity);
        }

        public void RemoveRange(IEnumerable<TEntity> entities)
        {
            DbContext.Set<TEntity>().RemoveRange(entities);
        }

        public void UpdateRange(IEnumerable<TEntity> entities)
        {
            DbContext.Set<TEntity>().UpdateRange(entities);
        }

        public void Update(TEntity entity)
        {
            DbContext.Set<TEntity>().UpdateRange(entity);
        }

        public Task<TEntity> FindByIdAsync(TIdentity id)
        {
            return DbContext.Set<TEntity>().SingleOrDefaultAsync(e => e.Id == id);
        }

        public async Task<TEntity> FindOneBySpecAsync(ISpecification<TEntity> spec)
        {
            var specificationResult = GetQuery(DbContext.Set<TEntity>(), spec);

            return await specificationResult.FirstOrDefaultAsync();
        }

        public async Task<List<TEntity>> FindBySpecAsync(ISpecification<TEntity> spec)
        {
            var specificationResult = GetQuery(DbContext.Set<TEntity>(), spec);

            return await specificationResult.ToListAsync();
        }

        public async ValueTask<long> CountAsync(ISpecification<TEntity> spec)
        {
            spec.IsPagingEnabled = false;
            var specificationResult = GetQuery(DbContext.Set<TEntity>(), spec);

            return await ValueTask.FromResult(specificationResult.LongCount());
        }

        private static IQueryable<TEntity> GetQuery(IQueryable<TEntity> inputQuery,
            ISpecification<TEntity> specification)
        {
            var query = inputQuery;

            if (specification.Criteria is not null)
            {
                query = query.Where(specification.Criteria);
            }

            query = specification.Includes.Aggregate(query, (current, include) => current.Include(include));

            query = specification.IncludeStrings.Aggregate(query, (current, include) => current.Include(include));

            if (specification.OrderBy is not null)
            {
                query = query.OrderBy(specification.OrderBy);
            }
            else if (specification.OrderByDescending is not null)
            {
                query = query.OrderByDescending(specification.OrderByDescending);
            }

            if (specification.GroupBy is not null)
            {
                query = query.GroupBy(specification.GroupBy).SelectMany(x => x);
            }

            if (specification.IsPagingEnabled)
            {
                query = query.Skip(specification.Skip - 1)
                    .Take(specification.Take);
            }

            return query;
        }
    }

    public class RepositoryBase<TDbContext, TEntity> : 
        RepositoryBase<TDbContext, TEntity, Guid, AggregateId>
        where TEntity : AggregateRoot
        where TDbContext : DbContext, ISqlDbContext
    {
        protected RepositoryBase(TDbContext dbContext) : base(dbContext)
        {
        }
    }
}