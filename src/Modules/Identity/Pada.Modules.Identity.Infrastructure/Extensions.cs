using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Pada.Abstractions.Persistence.Mssql;
using Pada.Modules.Identity.Infrastructure.Persistence;

namespace Pada.Modules.Identity.Infrastructure
{
    public static class Extensions
    {
        private const string SectionName = "mssql";
        
        public static IServiceCollection AddIdentityInfrastructure(this IServiceCollection services,
            ConfigurationManager configuration,
            string sectionName = SectionName)
        {
            if (string.IsNullOrWhiteSpace(sectionName)) sectionName = SectionName;
            
            services.Configure<MssqlOptions>(configuration.GetSection(sectionName));
            var mssqlOptions = configuration.GetSection(sectionName).Get<MssqlOptions>();
            services.AddDbContext<AppIdentityDbContext>(optionsBuilder =>
                {
                    //optionsBuilder.EnableSensitiveDataLogging(true);
                    optionsBuilder.UseSqlServer(mssqlOptions.ConnectionString);
                });
            return services;
        }

    }
}