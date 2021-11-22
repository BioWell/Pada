using System;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Pada.Abstractions.Persistence;
using Pada.Abstractions.Persistence.Mssql;
using Pada.Infrastructure.Persistence.Mssql;
using Pada.Modules.Identity.Infrastructure.Persistence;
using Pada.Modules.Identity.Infrastructure.Services;

namespace Pada.Modules.Identity.Infrastructure
{
    public static class Extensions
    {
        private const string SectionName = nameof(MssqlOptions);
        
        public static IServiceCollection AddIdentityInfrastructure(this IServiceCollection services,
            ConfigurationManager configuration,
            string sectionName = SectionName)
        {
            if (string.IsNullOrWhiteSpace(sectionName)) sectionName = SectionName;
            
            services.Configure<MssqlOptions>(configuration.GetSection(sectionName));
            var mssqlOptions = configuration.GetSection(sectionName).Get<MssqlOptions>();
            
            services.AddMssqlPersistence<AppIdentityDbContext>(
                mssqlOptions.ConnectionString,
                    configurator: s => { s.AddRepository(typeof(Repository<>)); },
                    optionBuilder: options =>
                    {
                        // options.UseTriggers(triggerOptions => {
                        //     triggerOptions.AddTrigger<AuditTrigger>();
                        // });
                    })
                .AddScoped<IAppIdentityDbContext>(provider => provider.GetRequiredService<AppIdentityDbContext>());
            
            // if (string.IsNullOrWhiteSpace(sectionName)) sectionName = SectionName;
            //
            // services.Configure<MssqlOptions>(configuration.GetSection(sectionName));
            // var mssqlOptions = configuration.GetSection(sectionName).Get<MssqlOptions>();
            // services.AddDbContext<AppIdentityDbContext>(optionsBuilder =>
            //     {
            //         optionsBuilder.EnableSensitiveDataLogging(true);
            //         optionsBuilder.UseSqlServer(mssqlOptions.ConnectionString);
            //     })
            //     .AddScoped<IAppIdentityDbContext>(provider => provider.GetService<AppIdentityDbContext>())
            //     .AddIdentity<AppUser, AppRole>(identityOptions =>
            //     {
            //         identityOptions.Password.RequireDigit = false;
            //         identityOptions.Password.RequiredLength = 4;
            //         identityOptions.Password.RequireNonAlphanumeric = false;
            //         identityOptions.Password.RequireUppercase = false;
            //         identityOptions.Password.RequireLowercase = false;
            //         identityOptions.Password.RequiredUniqueChars = 0;
            //
            //         identityOptions.User.RequireUniqueEmail = true;
            //
            //         identityOptions.SignIn.RequireConfirmedEmail = true;
            //         // TO DO
            //         // identityOptions.Tokens.EmailConfirmationTokenProvider = "emailconf";
            //         identityOptions.Lockout.AllowedForNewUsers = true;
            //         identityOptions.Lockout.MaxFailedAccessAttempts = 3;
            //         identityOptions.Lockout.DefaultLockoutTimeSpan = TimeSpan.FromMinutes(1);
            //     })
            //     .AddEntityFrameworkStores<AppIdentityDbContext>()
            //     .AddDefaultTokenProviders();
            return services;
        }

        public static IServiceCollection AddRepository(this IServiceCollection services, Type repoType)
        {
            services.Scan(scan => scan
                .FromAssembliesOf(repoType)
                .AddClasses(classes =>
                    classes.AssignableTo(repoType)).As(typeof(IRepository<,,>)).WithScopedLifetime()
            );

            return services;
        }
    }
}