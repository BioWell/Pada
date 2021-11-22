using System;
using System.Reflection;
using DbUp;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Pada.Abstractions.Persistence.Mssql;
using Pada.Infrastructure.Exceptions;

namespace Pada.Infrastructure.Persistence.Mssql
{
    public static class Extensions
    {
        public static IServiceCollection AddMssqlPersistence<TContext>(
            this IServiceCollection services,
            string connection,
            Action<IServiceCollection> configurator = null,
            Action<DbContextOptionsBuilder> optionBuilder = null)
            where TContext : DbContext, ISqlDbContext, IDbFacadeResolver //, IDomainEventContext
        {
            if (string.IsNullOrWhiteSpace(connection)) throw CoreException.NullArgument(connection);

            services.AddDbContext<TContext>(options =>
                {
                    //adding all existing triggers dynamically 
                    options.EnableSensitiveDataLogging(true);
                    options.UseTriggers(triggerOptions => triggerOptions.AddAssemblyTriggers(typeof(TContext).Assembly));
                    options.UseSqlServer(connection, sqlOptions =>
                    {
                        sqlOptions.MigrationsAssembly(typeof(TContext).Assembly.GetName().Name);
                        sqlOptions.EnableRetryOnFailure(5, TimeSpan.FromSeconds(10), null);
                    });
                    optionBuilder?.Invoke(options);
                })
                .AddScoped<ISqlDbContext>(ctx => ctx.GetRequiredService<TContext>())
                .AddScoped<IDbFacadeResolver>(ctx => ctx.GetRequiredService<TContext>());
            
            configurator?.Invoke(services);
            
            services.AddScoped<IUnitOfWork, UnitOfWork>();
            
            return services;
        }

        public static class DbUpInitializer
        {
            public static void Initialize(string connection)
            {
                var upgrader =
                    DeployChanges.To
                        .SqlDatabase(connection)
                        .WithScriptsEmbeddedInAssembly(Assembly.GetExecutingAssembly())
                        .LogToConsole()
                        .Build();
                var result = upgrader.PerformUpgrade();
            }
        }
    }
}