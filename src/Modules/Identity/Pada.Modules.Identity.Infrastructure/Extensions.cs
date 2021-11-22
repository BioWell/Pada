using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Pada.Modules.Identity.Infrastructure
{
    public static class Extensions
    {
        public static IServiceCollection AddIdentityInfrastructure(this IServiceCollection services,
            ConfigurationManager configuration)
        {
            // var options = services.GetOptions<MsSqlSettings>(nameof(MsSqlSettings));
            // services.AddDbContext<UsersDbContext>(optionsBuilder =>
            //     {
            //         //optionsBuilder.EnableSensitiveDataLogging(true);
            //         optionsBuilder.UseSqlServer(options.ConnectionString);
            //     });
            return services;
        }

    }
}