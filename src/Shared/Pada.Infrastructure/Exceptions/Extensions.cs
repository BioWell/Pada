using System;
using Humanizer;
using Microsoft.Extensions.DependencyInjection;
using Pada.Abstractions.Exceptions;

namespace Pada.Infrastructure.Exceptions
{
    public static class Extensions
    {
        public static string GetExceptionCode(this Exception exception)
        {
            return exception.GetType().Name.Underscore().Replace("_exception", string.Empty);
        }
        
        public static IServiceCollection AddExceptionToMessageMapper<T>(this IServiceCollection services)
            where T : class, IExceptionToMessageMapper
        {
            services.AddSingleton<T>();
            services.AddSingleton<IExceptionToMessageMapper, T>();

            return services;
        }
    }
}