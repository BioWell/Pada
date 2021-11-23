using System.Collections.Generic;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging;
using Pada.Modules.Identity.Infrastructure.Aggregates.Roles;

namespace Pada.Modules.Identity.Infrastructure.Services.Roles
{
    public class CustomRoleManager : AspNetRoleManager<AppRole>
    {
        public CustomRoleManager(IRoleStore<AppRole> store, 
            IEnumerable<IRoleValidator<AppRole>> roleValidators,
            ILookupNormalizer keyNormalizer, 
            IdentityErrorDescriber errors, 
            ILogger<CustomRoleManager> logger,
            IHttpContextAccessor contextAccessor) 
            : base(store, roleValidators, keyNormalizer, errors, logger, contextAccessor)
        {
        }
    }
}