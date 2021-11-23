using System.Reflection;
using MediatR;
using Microsoft.Extensions.DependencyInjection;

namespace Pada.Modules.Identity.Application
{
    public static class Extensions
    {
        public static IServiceCollection AddIdentityApplication(this IServiceCollection services)
        {
            services.AddAutoMapper(Assembly.GetExecutingAssembly());
            services.AddMediatR(Assembly.GetExecutingAssembly());
            // services.AddMediatR(typeof(GetUserByIdQueryHandler).Assembly);
            // services.AddMediatR(typeof(RegisterNewUserCommandHandler).Assembly);
            return services;
        }
    }
}