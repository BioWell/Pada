using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Pada.Abstractions.Services.Storage;
using Pada.Abstractions.Services.Token;
using Pada.Infrastructure.Services.Storage;
using Pada.Infrastructure.Services.Token;

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
            
            services.AddSingleton<IRequestStorage, RequestStorage>();
            services.AddSingleton<ITokenStorageService, TokenStorageService>();
        }
    }
}