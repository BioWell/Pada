using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using EntityFramework.Exceptions.SqlServer;
using Pada.Abstractions.Persistence;
using Pada.Modules.Identity.Infrastructure.Aggregates.Roles;
using Pada.Modules.Identity.Infrastructure.Aggregates.Users;
using Pada.Modules.Identity.Infrastructure.Persistence.Configurations;

namespace Pada.Modules.Identity.Infrastructure.Persistence
{
    public sealed class AppIdentityDbContext : IdentityDbContext<
            AppUser, AppRole, string,
            IdentityUserClaim<string>,
            AppUserRole, IdentityUserLogin<string>,
            IdentityRoleClaim<string>, IdentityUserToken<string>>,
        IAppIdentityDbContext
    {
        public AppIdentityDbContext(DbContextOptions<AppIdentityDbContext> options) : base(options)
        {
        }
        
        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            //https://github.com/Giorgi/EntityFramework.Exceptions
            optionsBuilder.UseExceptionProcessor();
        }
        
        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);
            
            builder.ApplyConfiguration(new AppRoleCfg());
            builder.ApplyConfiguration(new AppUserCfg());

            builder.Entity<IdentityUserClaim<string>>().Property(x => x.UserId).HasMaxLength(128);
            builder.Entity<IdentityUserLogin<string>>().Property(x => x.UserId).HasMaxLength(128);
            builder.Entity<IdentityUserLogin<string>>().Property(x => x.LoginProvider).HasMaxLength(128);
            builder.Entity<IdentityUserLogin<string>>().Property(x => x.ProviderKey).HasMaxLength(128);
            builder.Entity<AppUserRole>().Property(x => x.UserId).HasMaxLength(128);
            builder.Entity<AppUserRole>().Property(x => x.RoleId).HasColumnName("RoleCode").HasMaxLength(50);
            builder.Entity<IdentityRoleClaim<string>>().Property(x => x.RoleId).HasColumnName("RoleCode")
                .HasMaxLength(50);
            builder.Entity<IdentityUserToken<string>>().Property(x => x.UserId).HasMaxLength(128);

            MapsTables(builder);
        }
        
        private static void MapsTables(ModelBuilder builder)
        {
            builder.Entity<AppUser>(b => { b.ToTable("User"); }).HasDefaultSchema("identities");
            builder.Entity<IdentityUserClaim<string>>(b => { b.ToTable("UserClaim"); }).HasDefaultSchema("identities");
            builder.Entity<IdentityUserLogin<string>>(b => { b.ToTable("UserLogin"); }).HasDefaultSchema("identities");
            builder.Entity<IdentityUserToken<string>>(b => { b.ToTable("UserToken"); }).HasDefaultSchema("identities");
            builder.Entity<AppRole>(b => { b.ToTable("Role"); }).HasDefaultSchema("identities");
            builder.Entity<AppUserRole>(b => { b.ToTable("UserRoles"); }).HasDefaultSchema("identities");
            builder.Entity<IdentityRoleClaim<string>>(b => { b.ToTable("RoleClaim"); }).HasDefaultSchema("identities");
        }

        public DbSet<OutboxMessage> OutboxMessages { get; set; }
        
        public Task BeginTransactionAsync()
        {
            throw new System.NotImplementedException();
        }

        public Task RollbackTransaction()
        {
            throw new System.NotImplementedException();
        }
    }
}