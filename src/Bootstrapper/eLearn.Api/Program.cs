using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Versioning;
using Microsoft.Extensions.DependencyInjection;
using Pada.Infrastructure;
using Pada.Infrastructure.Modules;

var builder = WebApplication.CreateBuilder(args);
var configuration = builder.Configuration;

builder.ConfigureModules();
var assemblies = ModuleLoader.LoadAssemblies(configuration);
var modules = ModuleLoader.LoadModules(assemblies);

builder.AddModularInfrastructure(configuration, modules, assemblies);
AddCustomVersioning();
builder.Build().UseModularInfrastructure(modules).Run();

void AddCustomVersioning()
{
    builder.Services.AddApiVersioning(options =>
    {
        options.ReportApiVersions = true;
        options.AssumeDefaultVersionWhenUnspecified = true;
        options.DefaultApiVersion = new ApiVersion(1, 0);
        options.ApiVersionReader = ApiVersionReader.Combine(new HeaderApiVersionReader("api-version"),
            new UrlSegmentApiVersionReader());
    });
}