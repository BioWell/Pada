using Microsoft.Extensions.DependencyInjection;

namespace Pada.Infrastructure.Exceptions
{
    public static class Extensions
    {
        public static IServiceCollection AddCoreExcepitons(this IServiceCollection services)
        {
            services.AddSingleton<GlobalExceptionHandler>();
            return services;
        }
    }
}