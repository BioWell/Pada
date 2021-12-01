using System.Collections.Generic;
using System.Security.Claims;

namespace Pada.Infrastructure.Auth
{
    public class ClaimPolicy
    {
        public string Name { get; set; }
        public IList<Claim> Claims { get; set; }
    }
}