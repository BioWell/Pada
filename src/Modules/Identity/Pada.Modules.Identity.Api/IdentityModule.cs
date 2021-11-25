using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Pada.Abstractions.Modules;
using Pada.Modules.Identity.Application;
using Pada.Modules.Identity.Infrastructure;

namespace Pada.Modules.Identity.Api
{
    public class IdentityModule : IModule
    {
        public const string ModulePath = "identity";
        public string Name => "Identity";
        public string Path => ModulePath;
        public ConfigurationManager Configuration { get; private set; } = null!;

        public void Init(ConfigurationManager configuration)
        {
            Configuration = configuration;
        }

        public void Register(IServiceCollection services)
        {
            services.AddIdentityInfrastructure(Configuration, Name);
            services.AddIdentityApplication();
        }

        public void Use(IApplicationBuilder app, IWebHostEnvironment environment)
        {
        }

        public void EndpointsConfigure(IEndpointRouteBuilder endpoints)
        {
            endpoints.MapGet(Path, ctx => ctx.Response.WriteAsync($"{Name} module"));
            endpoints.MapGet($"{Path}/ping", ctx => ctx.Response.WriteAsJsonAsync(true));
        }
    }
}