using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Pada.Infrastructure.App;
using Pada.Infrastructure.Logging.Options;
using Serilog;
using Serilog.Events;
using Serilog.Filters;

namespace Pada.Infrastructure.Logging
{
    public static class Extensions
    {
        private const string ConsoleOutputTemplate = 
            "{Timestamp:HH:mm:ss} [{Level:u3}] {Message}{NewLine}{Exception}";

        private const string FileOutputTemplate =
            "{Timestamp:HH:mm:ss} [{Level:u3}] ({SourceContext}.{Method}) {Message}{NewLine}{Exception}";
        
        private const string LoggerSectionName = nameof(LoggerOptions);
        private const string AppSectionName = nameof(AppOptions);
        
        public static IHostBuilder UseLogging(this IHostBuilder builder,
            Action<LoggerConfiguration> configure = null,
            string loggerSectionName = LoggerSectionName,
            string appSectionName = AppSectionName)
            => builder.UseSerilog((context, loggerConfiguration) =>
            {
                if (string.IsNullOrWhiteSpace(loggerSectionName)) loggerSectionName = LoggerSectionName;
                var loggerOptions = context.Configuration.GetSection(loggerSectionName).Get<LoggerOptions>();
                
                if (string.IsNullOrWhiteSpace(loggerSectionName)) appSectionName = AppSectionName;
                var appOptions = context.Configuration.GetSection(appSectionName).Get<AppOptions>();
                
                MapOptions(loggerOptions, appOptions, loggerConfiguration, context.HostingEnvironment.EnvironmentName);
                configure?.Invoke(loggerConfiguration);
            });
        
        private static void MapOptions(LoggerOptions loggerSettings, 
            AppOptions appOptions,
            LoggerConfiguration loggerConfiguration, 
            string environmentName)
        {
            var level = GetLogEventLevel(loggerSettings.Level);

            loggerConfiguration.Enrich.FromLogContext()
                .MinimumLevel.Is(level)
                .Enrich.WithProperty("Environment", environmentName)
                .Enrich.WithProperty("Application", appOptions.Name)
                .Enrich.WithProperty("Instance", appOptions.Instance)
                .Enrich.WithProperty("Version", appOptions.Version);

            foreach (var (key, value) in loggerSettings.Tags ?? new Dictionary<string, object>())
            {
                loggerConfiguration.Enrich.WithProperty(key, value);
            }

            foreach (var (key, value) in loggerSettings.Overrides ?? new Dictionary<string, string>())
            {
                var logLevel = GetLogEventLevel(value);
                loggerConfiguration.MinimumLevel.Override(key, logLevel);
            }

            loggerSettings.ExcludePaths?.ToList().ForEach(p => loggerConfiguration.Filter
                .ByExcluding(Matching.WithProperty<string>("RequestPath", n => n.EndsWith(p))));

            loggerSettings.ExcludeProperties?.ToList().ForEach(p => loggerConfiguration.Filter
                .ByExcluding(Matching.WithProperty(p)));

            Configure(loggerConfiguration, loggerSettings);
        }
        
        private static void Configure(LoggerConfiguration loggerConfiguration, LoggerOptions settings)
        {
            var consoleOptions = settings.Console ?? new ConsoleOptions();
            var fileOptions = settings.Files ?? new FilesOptions();
            var seqOptions = settings.Seq ?? new SeqOptions();

            if (consoleOptions.Enabled)
            {
                loggerConfiguration.WriteTo.Console(outputTemplate: ConsoleOutputTemplate);
            }

            if (fileOptions.Enabled)
            {
                var path = string.IsNullOrWhiteSpace(fileOptions.Path) ? "logs/logs.txt" : fileOptions.Path;
                if (!Enum.TryParse<RollingInterval>(fileOptions.Interval, true, out var interval))
                {
                    interval = RollingInterval.Day;
                }

                loggerConfiguration.WriteTo.File(path, rollingInterval: interval, outputTemplate: FileOutputTemplate);
            }

            if (seqOptions.Enabled)
            {
                loggerConfiguration.WriteTo.Seq(seqOptions.Url, apiKey: seqOptions.ApiKey);
            }
        }
        
        private static LogEventLevel GetLogEventLevel(string level)
            => Enum.TryParse<LogEventLevel>(level, true, out var logLevel)
                ? logLevel
                : LogEventLevel.Information;
    }
}