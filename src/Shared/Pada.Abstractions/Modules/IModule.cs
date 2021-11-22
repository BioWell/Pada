using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Pada.Abstractions.Modules
{
    public interface IModule
    {
        string Name { get; }
        string Path { get; }
        void Init(ConfigurationManager configuration);
        void Register(IServiceCollection services);
        void Use(IApplicationBuilder app, IWebHostEnvironment environment);
        void EndpointsConfigure(IEndpointRouteBuilder endpoints);
    }
}