using System;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace Pada.Infrastructure.Utils
{
    public static class Common
    {
        public static bool IsLocal(this IHostEnvironment hostEnvironment) =>
            hostEnvironment?.IsEnvironment("local") ??
            throw new ArgumentNullException(nameof(hostEnvironment));
        
        public static string GetModuleName(this object value)
            => value?.GetType().GetModuleName() ?? string.Empty;
        
        public static string GetModuleName(this Type type, string namespacePart = "Modules", int splitIndex = 2)
        {
            if (type?.Namespace is null)
            {
                return string.Empty;
            }

            return type.Namespace.Contains(namespacePart)
                ? type.Namespace.Split(".")[splitIndex].ToLowerInvariant()
                : string.Empty;
        }
        
        public static T GetOptions<T>(this IServiceCollection services, string sectionName) where T : new()
        {
            using var serviceProvider = services.BuildServiceProvider();
            var configuration = serviceProvider.GetRequiredService<IConfiguration>();
            return configuration.GetOptions<T>(sectionName);
        }

        public static T GetOptions<T>(this IConfiguration configuration, string sectionName) where T : new()
        {
            var options = new T();
            configuration.GetSection(sectionName).Bind(options);
            return options;
        }
    }
}