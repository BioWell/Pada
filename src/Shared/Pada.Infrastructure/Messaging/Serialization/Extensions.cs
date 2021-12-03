using System;
using Microsoft.Extensions.DependencyInjection;
using Pada.Abstractions.Messaging.Serialization;

namespace Pada.Infrastructure.Messaging.Serialization
{
    public static class Extensions
    {
        public static IServiceCollection AddNewtonsoftMessageSerializer(this IServiceCollection services,
            Action<NewtonsoftJsonOptions> newtonsoftJsonOptions = null)
        {
            services.Configure(newtonsoftJsonOptions);
            services.AddSingleton<NewtonsoftJsonUnSupportedTypeMatcher>();
            services.AddSingleton<IMessageSerializer, MessageSerializer>();

            return services;
        }
    }
}