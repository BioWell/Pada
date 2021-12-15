using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Pada.Abstractions.Auth;
using Pada.Abstractions.Modules;
using Pada.Infrastructure.Auth;
using Pada.Infrastructure.Web.Extensions;
using Pada.Modules.Identity.Application;
using Pada.Modules.Identity.Domain.Aggregates.Users;
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
            services.AddCustomJwtAuthentication(Configuration,
                GetClaimPolicies(),
                GetRolePolicies(),
                TokenStorageType.InMemory,
                nameof(JwtOptions));
            services.AddIdentityApplication();
        }

        public void Use(IApplicationBuilder app, IWebHostEnvironment environment)
        {
        }

        public void EndpointsConfigure(IEndpointRouteBuilder endpoints)
        {
            endpoints.MapGet(Path, ctx => ctx.Response.WriteAsync($"{Name} module"));
            endpoints.MapGet($"{Path}/protected",
                    ctx => ctx.Response.WriteAsync($"{Name} module, protected end-point works."))
                .ApplyPoliciesAuthorization(new List<string>()
                {
                    AppPermission.Users.Create.Name
                });
            endpoints.MapGet($"{Path}/ping", ctx => ctx.Response.WriteAsJsonAsync(true));
        }

        private IList<ClaimPolicy> GetClaimPolicies()
        {
            return AppPermission.GetAllPermissions().Select(x => new ClaimPolicy
            {
                Name = x.Name,
                Claims = new List<Claim>
                {
                    new Claim(CustomClaimTypes.Permission, x.Name)
                }
            }).ToList();
        }

        private IList<RolePolicy> GetRolePolicies()
        {
            return Role.AllRoles().Select(x => new RolePolicy()
            {
                Name = x.Name,
                Roles = new List<string> {x.Name}
            }).ToList();
        }
    }
}