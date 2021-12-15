using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Builder;

namespace Pada.Infrastructure.Web.Extensions;

public static class EndpointConventionBuilder
{
    public static void ApplyPoliciesAuthorization(this IEndpointConventionBuilder builder,
        IList<string> policies)
    {
        if (policies is not null && policies.Any())
        {
            builder.RequireAuthorization(policies.ToArray());
        }
    }

    public static void ApplyRolesAuthorization(this IEndpointConventionBuilder builder,
        IList<string> roles)
    {
        if (roles is not null && roles.Any())
        {
            var authorize = new AuthorizeAttribute {Roles = string.Join(",", roles)};
            builder.RequireAuthorization(authorize);
        }
    }
}