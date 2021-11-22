using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Pada.Abstractions.Persistence;

namespace Pada.Infrastructure.Persistence.Mssql
{
    public class OutboxMessageCfg: IEntityTypeConfiguration<OutboxMessage>
    {
        public void Configure(EntityTypeBuilder<OutboxMessage> builder)
        {
            builder.ToTable("OutboxMessage", "dbo");

            builder.HasKey(e => e.Id);
        }
    }
}