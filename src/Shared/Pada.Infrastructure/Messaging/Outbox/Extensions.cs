using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Pada.Abstractions.Messaging;
using Pada.Abstractions.Messaging.Outbox;
using Pada.Abstractions.Persistence.Mssql;

namespace Pada.Infrastructure.Messaging.Outbox
{
    public static class Extensions
    {
        private const string SectionName = nameof(OutboxOptions);

        public static IServiceCollection AddEntityFrameworkOutbox<TContext>(this IServiceCollection services,
            IConfiguration configuration,
            string sectionName = SectionName)
            where TContext : DbContext, ISqlDbContext
        {
            var outboxOptions = configuration.GetSection(sectionName).Get<OutboxOptions>();
            services.AddOptions<OutboxOptions>()
                .Bind(configuration.GetSection(sectionName))
                .ValidateDataAnnotations();

            services.AddTransient<IOutbox, EFCoreOutbox<TContext>>();

            // Adding background service
            if (outboxOptions.Enabled)
                services.AddHostedService<OutboxProcessorBackgroundService>();

            services.AddSingleton<INotificationEventDispatcher, NotificationEventDispatcher>();
            return services;
        }
    }
}