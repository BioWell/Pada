using System.Collections.Generic;
using System.Threading.Tasks;
using Pada.Abstractions.Domain.Types;
using Pada.Abstractions.Persistence.Specification;

namespace Pada.Abstractions.Persistence
{
    public interface IRepository<TEntity, TId, in TIdentity>
        where TEntity : AggregateRoot<TId, TIdentity>
        where TIdentity : IdentityBase<TId>
    {
        IAsyncEnumerable<TEntity> FindAllAsync(ISpecification<TEntity> specification = null);
        Task<TEntity> FindOneBySpecAsync(ISpecification<TEntity> spec);
        Task<List<TEntity>> FindBySpecAsync(ISpecification<TEntity> spec);
        Task AddRangeAsync(IEnumerable<TEntity> entities);
        void Update(TEntity entity);
        void UpdateRange(IEnumerable<TEntity> entities);
        void Remove(TEntity entity);
        void RemoveRange(IEnumerable<TEntity> entities);
        Task<TEntity> FindByIdAsync(TIdentity id);
        ValueTask<long> CountAsync(ISpecification<TEntity> spec);
    }
}