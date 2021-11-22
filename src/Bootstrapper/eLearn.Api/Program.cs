using System.Linq;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Pada.Infrastructure.Modules;
using Pada.Infrastructure.Web.Extensions;

var builder = WebApplication.CreateBuilder(args);
var configuration = builder.Configuration;
var services = builder.Services;
builder.ConfigureModules();
var assemblies = ModuleLoader.LoadAssemblies(configuration);
var modules = ModuleLoader.LoadModules(assemblies);
modules.ToList().ForEach(x => x.Init(configuration));

services.AddWebApi(configuration);
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
foreach (var module in modules) module.ConfigureServices(services);

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

// app.UseHttpsRedirection();
app.UseRouting();
app.UseAuthorization();
foreach (var module in modules)
{
    app.Logger.LogInformation($"Configuring the middleware for: '{module.Name} module'...");
    module.Configure(app, app.Environment);
}

app.UseEndpoints(endpoints =>
{
    endpoints.MapControllers(); //.RequireAuthorization(); //enforce all controller tp authorize. we use AllowAnonymous attribute to bypass this rule
    endpoints.MapGet("/", context => context.Response.WriteAsync("Online Store API!"));
    foreach (var module in modules)
    {
        app.Logger.LogInformation($"Configuring the endpoints for: '{module.Name} module', path: '/{module.Path}'...");
        module.ConfigureEndpoints(endpoints);
    }
});
app.Run();