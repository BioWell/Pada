using System.Collections.Generic;

namespace Pada.Infrastructure.Auth
{
    public class RolePolicy
    {
        public string Name { get; set; }
        public IList<string> Roles { get; set; }
    }
}