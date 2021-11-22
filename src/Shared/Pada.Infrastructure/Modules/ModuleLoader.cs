using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Configuration;
using Pada.Abstractions.Modules;

namespace Pada.Infrastructure.Modules
{
    public static class ModuleLoader
    {
        const string ModulePart = "Pada.Modules.";
        
        public static WebApplicationBuilder ConfigureModules(this WebApplicationBuilder builder)
        {
            foreach (var settings in GetSettings("*"))
            {
                builder.Configuration.AddJsonFile(settings);
            }
            
            foreach (var settings in GetSettings($"*.{builder.Environment.EnvironmentName}"))
            {
                builder.Configuration.AddJsonFile(settings);
            }
            
            IEnumerable<string> GetSettings(string pattern)
                => Directory.EnumerateFiles(builder.Environment.ContentRootPath,
                    $"module.{pattern}.json", SearchOption.AllDirectories);

            return builder;
        }
        
        public static IList<Assembly> LoadAssemblies(ConfigurationManager configuration)
        {
            var assemblies = AppDomain.CurrentDomain.GetAssemblies()
                .Where(x => x.FullName?.Contains(ModulePart) ?? false)
                .ToList();
            var locations = assemblies.Where(x => !x.IsDynamic).Select(x => x.Location).ToArray();
            var files = Directory.GetFiles(AppDomain.CurrentDomain.BaseDirectory, "*.dll")
                .Where(x => !locations.Contains(x, StringComparer.InvariantCultureIgnoreCase))
                .ToList();

            var disabledModules = new List<string>();
            foreach (var file in files)
            {
                if (Path.GetFileName(file).Contains(ModulePart) == false)
                    continue;

                var moduleName = file.Split(ModulePart)[1].Split(".")[0].ToLowerInvariant();
                var enabled = configuration.GetValue(typeof(bool), $"{moduleName}:module:enabled");
                if (enabled is not null && (bool) enabled == false)
                {
                    disabledModules.Add(file);
                }

                assemblies.Add(AppDomain.CurrentDomain.Load(AssemblyName.GetAssemblyName(file)));
            }

            foreach (var disabledModule in disabledModules)
            {
                files.Remove(disabledModule);
            }

            return assemblies;
        }

        public static IList<IModule> LoadModules(IEnumerable<Assembly> assemblies)
            => assemblies
                .SelectMany(x => x.GetTypes())
                .Where(x => typeof(IModule).IsAssignableFrom(x) && !x.IsInterface)
                .OrderBy(x => x.Name)
                .Select(Activator.CreateInstance)
                .Cast<IModule>()
                .ToList();
    }
}