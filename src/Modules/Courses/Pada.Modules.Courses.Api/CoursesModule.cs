using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Pada.Abstractions.Modules;

namespace Pada.Modules.Courses.Api
{
    public class CoursesModule : IModule
    {
        public const string ModulePath = "courses";
        public string Name => "Courses";
        public string Path => ModulePath;
        public ConfigurationManager Configuration { get; private set; } = null!;

        public void Init(ConfigurationManager configuration)
        {
            Configuration = configuration;
        }

        public void Register(IServiceCollection services)
        {
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