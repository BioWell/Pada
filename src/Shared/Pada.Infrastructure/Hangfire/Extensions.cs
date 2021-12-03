using Hangfire;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Newtonsoft.Json;
using Pada.Abstractions.Services.Hangfire;
using Pada.Infrastructure.Services.Hangfire;

namespace Pada.Infrastructure.Hangfire
{
    public static class Extensions
    {
        public const string SectionName = nameof(HangfireOptions);

        public static IServiceCollection AddHangfireScheduler(this IServiceCollection services,
            IConfiguration configuration,
            string sectionName = SectionName)
        {
            var options = configuration.GetSection(sectionName).Get<HangfireOptions>();
            services.AddOptions<HangfireOptions>().Bind(configuration.GetSection(sectionName))
                .ValidateDataAnnotations();

            var jsonSettings = new JsonSerializerSettings
            {
                TypeNameHandling = TypeNameHandling.All
            };

            services.AddHangfire(hangfireConfiguration =>
            {
                hangfireConfiguration.UseSqlServerStorage(options.ConnectionString);
                hangfireConfiguration.UseSerializerSettings(jsonSettings);
            });
            services.AddHangfireServer();

            services.AddScoped<IJobService, HangfireService>();  
            
            // services.AddScoped<IHangfireMessagesScheduler, HangfireMessagesScheduler>();
            // services.AddScoped<IMessagesScheduler, HangfireMessagesScheduler>();

            return services;
        }

        public static IApplicationBuilder UseHangfireScheduler(this IApplicationBuilder app)
        {
            return app.UseHangfireDashboard("/mydashboard");
        }
    }
}