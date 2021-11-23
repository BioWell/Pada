using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Options;
using Pada.Modules.Identity.Infrastructure.Aggregates.Roles;
using Pada.Modules.Identity.Infrastructure.Aggregates.Users;

namespace Pada.Modules.Identity.Infrastructure.Services.Users
{
    public class CustomUserClaimsPrincipalFactory : UserClaimsPrincipalFactory<AppUser, AppRole>
    {
        public CustomUserClaimsPrincipalFactory(UserManager<AppUser> userManager,
            RoleManager<AppRole> roleManager, 
            IOptions<IdentityOptions> options) 
            : base(userManager, roleManager, options)
        {
        }
    }
}