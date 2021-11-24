using System.Reflection;
using FluentValidation.AspNetCore;
using MediatR;
using Microsoft.Extensions.DependencyInjection;

namespace Pada.Modules.Identity.Application
{
    public static class Extensions
    {
        public static IServiceCollection AddIdentityApplication(this IServiceCollection services)
        {
            return services;
        }
    }
}