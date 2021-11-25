using System;
using System.Reflection;
using MediatR;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Pada.Abstractions.Persistence.Mssql;
using Pada.Infrastructure.Persistence.Mssql;
using Pada.Infrastructure.Utils;
using Pada.Modules.Identity.Application;
using Pada.Modules.Identity.Application.Users.Contracts;
using Pada.Modules.Identity.Application.Users.Features.RegisterNewUser;
using Pada.Modules.Identity.Infrastructure.Aggregates.Roles;
using Pada.Modules.Identity.Infrastructure.Aggregates.Users;
using Pada.Modules.Identity.Infrastructure.Persistence;
using Pada.Modules.Identity.Infrastructure.Services.Roles;
using Pada.Modules.Identity.Infrastructure.Services.Users;

namespace Pada.Modules.Identity.Infrastructure
{
    public static class Extensions
    {
        private const string MssqlSectionName = nameof(MssqlOptions);
        
        private const string IdentitySectionName = nameof(IdentityOptions);

        public static IServiceCollection AddIdentityInfrastructure(this IServiceCollection services,
            ConfigurationManager configuration,
            string moduleName,
            string mssqlSectionName = MssqlSectionName,
            string identitySectionName = IdentitySectionName)
        {
            if (string.IsNullOrWhiteSpace(mssqlSectionName))
                mssqlSectionName = MssqlSectionName;
            
            var registrationOptions =
                services.GetOptions<RegistrationOptions>($"{moduleName}:" + nameof(RegistrationOptions));
            services.AddSingleton(registrationOptions);

            services.Configure<MssqlOptions>(configuration.GetSection(mssqlSectionName));
            var mssqlOptions = configuration.GetSection(mssqlSectionName).Get<MssqlOptions>();

            services.AddMssqlPersistence<AppIdentityDbContext>(mssqlOptions.ConnectionString)
                .AddScoped<IAppIdentityDbContext>(provider => provider.GetRequiredService<AppIdentityDbContext>());

            services.TryAddTransient<IUserRepository, UserRepository>();
            
            //Identity dependencies override
            services.TryAddScoped<RoleManager<AppRole>, CustomRoleManager>();
            services.TryAddSingleton<Func<RoleManager<AppRole>>>(provider =>
                () => provider.CreateScope().ServiceProvider.GetRequiredService<RoleManager<AppRole>>());
            
            services.TryAddScoped<UserManager<AppUser>, CustomUserManager>();
            services.TryAddSingleton<Func<UserManager<AppUser>>>(provider =>
                () => provider.CreateScope().ServiceProvider.GetRequiredService<UserManager<AppUser>>());
            
            services.TryAddSingleton<Func<SignInManager<AppUser>>>(provider =>
                () => provider.CreateScope().ServiceProvider.GetRequiredService<SignInManager<AppUser>>());
            
            services.TryAddScoped<IUserClaimsPrincipalFactory<AppUser>, CustomUserClaimsPrincipalFactory>();
            
            services.AddIdentity<AppUser, AppRole>(options => options.Stores.MaxLengthForKeys = 128)
                .AddEntityFrameworkStores<AppIdentityDbContext>()
                .AddUserManager<CustomUserManager>()
                .AddRoleManager<CustomRoleManager>()
                .AddDefaultTokenProviders();

            services.Configure<IdentityOptions>(options =>
            {
                options.Password.RequireDigit = false;
                options.Password.RequiredLength = 6;
                options.Password.RequireLowercase = false;
                options.Password.RequireNonAlphanumeric = false;
                options.Password.RequireUppercase = false;
                options.Lockout.AllowedForNewUsers = true;
                options.Lockout.DefaultLockoutTimeSpan = TimeSpan.FromMinutes(10);
                options.Lockout.MaxFailedAccessAttempts = 3;
            });
            
            if (string.IsNullOrWhiteSpace(identitySectionName))
                identitySectionName = IdentitySectionName;
            
            services.Configure<IdentityOptions>(configuration.GetSection(identitySectionName));
            
            return services;
        }
    }
}