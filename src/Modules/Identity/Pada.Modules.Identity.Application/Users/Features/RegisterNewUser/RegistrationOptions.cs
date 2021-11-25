using System.Collections.Generic;

namespace Pada.Modules.Identity.Application.Users.Features.RegisterNewUser
{
    public class RegistrationOptions
    {
        public bool Enabled { get; set; }
        public IEnumerable<string> InvalidEmailProviders { get; set; } = System.Array.Empty<string>();
    }
}