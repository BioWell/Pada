using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Pada.Infrastructure.Caching
{
    public static class Extensions
    {
        public static void AddCaching(this IServiceCollection services, ConfigurationManager configuration)
        {
            services.AddEasyCaching(options =>
            {
                options.UseInMemory(configuration, "mem");
                // options.UseRedis(configuration, "redis").WithMessagePack();
            });
        }
    }
}