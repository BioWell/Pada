using System;
using Figgle;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Pada.Infrastructure.App;

namespace Pada.Infrastructure.Web.Extensions
{
    public static class WebApiExtensions
    {
        private const string AppOptionsSectionName = nameof(AppOptions);

        public static IServiceCollection AddWebApi(this IServiceCollection services,
            ConfigurationManager configuration,
            string appOptionSection = AppOptionsSectionName)
        {
            services.AddHttpContextAccessor();
            
            var appOptions = configuration.GetSection(appOptionSection).Get<AppOptions>();
            services.AddOptions<AppOptions>().Bind(configuration.GetSection(appOptionSection))
                .ValidateDataAnnotations();
            
            services.AddControllers()
                .ConfigureApplicationPartManager(manager =>
                {
                    manager.FeatureProviders.Add(new InternalControllerFeatureProvider());
                });            
            
            if (!appOptions.DisplayBanner || string.IsNullOrWhiteSpace(appOptions.Name)) return services;

            var version = appOptions.DisplayVersion ? $" {appOptions.Version}" : string.Empty;
            Console.WriteLine(FiggleFonts.Doom.Render($"{appOptions.Name}{version}"));
            
            return services;
        }
    }
}