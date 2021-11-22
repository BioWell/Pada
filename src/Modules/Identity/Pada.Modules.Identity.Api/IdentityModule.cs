using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Pada.Abstractions.Modules;

namespace Pada.Modules.Identity.Api
{
    public class IdentityModule : IModule
    {
        public const string ModulePath = "identity";
        public string Name => "Identity";
        public string Path => ModulePath;
        public IConfiguration Configuration { get; private set; } = null!;

        public void Init(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public void ConfigureServices(IServiceCollection services)
        {
        }

        public void Configure(IApplicationBuilder app, IWebHostEnvironment environment)
        {
        }

        public void ConfigureEndpoints(IEndpointRouteBuilder endpoints)
        {
            endpoints.MapGet(Path, ctx => ctx.Response.WriteAsync($"{Name} module"));
            endpoints.MapGet($"{Path}/ping", ctx => ctx.Response.WriteAsJsonAsync(true));
        }
    }
}