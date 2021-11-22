using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Pada.Modules.Identity.Infrastructure.Aggregates.Roles;

namespace Pada.Modules.Identity.Infrastructure.Persistence.Configurations
{
    public class AppRoleCfg: IEntityTypeConfiguration<AppRole>
    {
        public void Configure(EntityTypeBuilder<AppRole> builder)
        {
            // Each Role can have many entries in the UserRole join table
            builder.HasMany(e => e.UserRoles)
                .WithOne(e => e.Role)
                .HasForeignKey(ur => ur.RoleId)
                .IsRequired();

            // this ignored properties will handle in UserManager and RoleManager
            builder.Ignore(x => x.Permissions);
            builder.Property(x => x.Id).HasColumnName("Code").HasMaxLength(50);
        }
    }
}