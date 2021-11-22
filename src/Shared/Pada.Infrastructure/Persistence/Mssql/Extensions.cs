using System;
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
            services.AddScoped<IUnitOfWork, UnitOfWork>();
            
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
            
            return services;
        }
    }
}