using System.Collections.Generic;
using Microsoft.AspNetCore.Identity;
using Pada.Modules.Identity.Domain.Aggregates.Users;
using Pada.Modules.Identity.Infrastructure.Aggregates.Users;

namespace Pada.Modules.Identity.Infrastructure.Aggregates.Roles
{
    public class AppRole: IdentityRole<string>
    {
        public AppRole()
        {
            Permissions = new List<AppPermission>();
        }

        public string Description { get; set; }
        public IList<AppPermission> Permissions { get; set; }
        public virtual ICollection<AppUserRole> UserRoles { get; set; }
    }
}