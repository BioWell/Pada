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
            string connection)
            where TContext : DbContext, ISqlDbContext, IDbFacadeResolver //, IDomainEventContext
        {
            if (string.IsNullOrWhiteSpace(connection)) throw new CoreException($"{connection} cannot be null");

            services.AddDbContext<TContext>(options =>
                {
                    // options.EnableSensitiveDataLogging(true);
                    options.UseTriggers(triggerOptions => triggerOptions.AddAssemblyTriggers(typeof(TContext).Assembly));
                    options.UseSqlServer(connection, sqlOptions =>
                    {
                        sqlOptions.MigrationsAssembly(typeof(TContext).Assembly.GetName().Name);
                        sqlOptions.EnableRetryOnFailure(5, TimeSpan.FromSeconds(10), null);
                    });
                })
                .AddScoped<ISqlDbContext>(ctx => ctx.GetRequiredService<TContext>())
                .AddScoped<IDbFacadeResolver>(ctx => ctx.GetRequiredService<TContext>());

            return services;
        }
    }
}