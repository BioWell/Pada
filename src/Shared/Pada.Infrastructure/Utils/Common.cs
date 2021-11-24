using System;
using FluentValidation.Results;
using Microsoft.Extensions.Hosting;
using Pada.Infrastructure.Validations;

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
    }
}