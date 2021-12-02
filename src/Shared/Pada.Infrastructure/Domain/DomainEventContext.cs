using System.Collections.Generic;
using Pada.Abstractions.Domain;

namespace Pada.Infrastructure.Domain
{
    public class DomainEventContext : IDomainEventContext
    {
        // private readonly ISqlDbContext _dbContext;
        
        public IEnumerable<IDomainEvent> GetDomainEvents()
        {
            throw new System.NotImplementedException();
        }
    }
}