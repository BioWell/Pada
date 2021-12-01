using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Pada.Abstractions.Services.Mail;
using Pada.Abstractions.Services.Sms;
using Pada.Infrastructure.Services.Mail;
using Pada.Infrastructure.Services.Sms;
using Pada.Infrastructure.Services.Sms.Models;

namespace Pada.Infrastructure.Services
{
    public static class Extensions
    {
        private const string AliyunSmsOptionsSectionName = nameof(AliyunSmsOptions);

        public static IServiceCollection AddServicesApplicationLayer(this IServiceCollection services,
            ConfigurationManager configuration,
            string appOptionSection = AliyunSmsOptionsSectionName)
        {
            services.Configure<AliyunSmsOptions>(configuration.GetSection(appOptionSection));
            services.AddOptions<AliyunSmsOptions>().Bind(configuration.GetSection(appOptionSection));
            services.AddScoped<ISmsSender, AliyunSmsSenderService>();
            
            services.Configure<MailOptions>(configuration.GetSection(nameof(MailOptions)));
            services.AddSingleton<ICustomMailService, SmtpMailService>();

            return services;
        }
    }
}