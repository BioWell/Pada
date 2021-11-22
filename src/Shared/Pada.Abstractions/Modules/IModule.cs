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
        void Init(IConfiguration configuration);
        void ConfigureServices(IServiceCollection services);
        void Configure(IApplicationBuilder app, IWebHostEnvironment environment);
        void ConfigureEndpoints(IEndpointRouteBuilder endpoints);
    }
}