using Microsoft.AspNetCore.Builder;
using Pada.Infrastructure;
using Pada.Infrastructure.Modules;

var builder = WebApplication.CreateBuilder(args);
var configuration = builder.Configuration;
var services = builder.Services;

builder.ConfigureModules();
var modules = ModuleLoader.LoadModules(ModuleLoader.LoadAssemblies(configuration));
services.AddModularInfrastructure(configuration, modules);
builder.Build().UseModularInfrastructure(modules).Run();