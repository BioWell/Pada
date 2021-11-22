using Microsoft.AspNetCore.Identity;
using Pada.Modules.Identity.Infrastructure.Aggregates.Roles;

namespace Pada.Modules.Identity.Infrastructure.Aggregates.Users
{
    public class AppUserRole: IdentityUserRole<string>
    {
        public virtual AppUser User { get; set; }
        public virtual AppRole Role { get; set; }
    }
}