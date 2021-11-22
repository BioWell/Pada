using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Pada.Modules.Identity.Infrastructure.Aggregates.Users;

namespace Pada.Modules.Identity.Infrastructure.Persistence.Configurations
{
    public class AppUserCfg : IEntityTypeConfiguration<AppUser>
    {
        public void Configure(EntityTypeBuilder<AppUser> builder)
        {
            // Each User can have many UserClaims
            builder.HasMany(e => e.Claims)
                .WithOne()
                .HasForeignKey(uc => uc.UserId)
                .IsRequired();

            // Each User can have many UserLogins
            builder.HasMany(e => e.Logins)
                .WithOne()
                .HasForeignKey(ul => ul.UserId)
                .IsRequired();

            // Each User can have many UserTokens
            builder.HasMany(e => e.Tokens)
                .WithOne()
                .HasForeignKey(ut => ut.UserId)
                .IsRequired();

            // Each User can have many entries in the UserRole join table
            builder.HasMany(e => e.UserRoles)
                .WithOne(e => e.User)
                .HasForeignKey(ur => ur.UserId)
                .IsRequired();

            builder.OwnsMany(x => x.RefreshTokens, b =>
            {
                b.Property<int>("Id")
                    .ValueGeneratedOnAdd()
                    .HasColumnType("int")
                    .HasAnnotation("SqlServer:ValueGenerationStrategy",
                        SqlServerValueGenerationStrategy.IdentityColumn);
                b.HasKey("Id");

                b.WithOwner().HasForeignKey("UserId");
                b.Property<string>("UserId");
                b.ToTable("RefreshToken", "Identities");
            });

            builder.Ignore(x => x.Password);

            // this ignored properties will handle in UserManager and RoleManager
            builder.Ignore(x => x.Permissions);
            builder.Ignore(x => x.Roles);
            builder.Ignore(x => x.Logins);

            builder.Property(x => x.UserType).HasMaxLength(64);
            builder.Property(x => x.PhotoUrl).HasMaxLength(2048);
            builder.Property(x => x.Id).HasMaxLength(128).ValueGeneratedOnAdd();
        }
    }
}