using System;
using System.Collections.Generic;

namespace Pada.Abstractions.Auth
{
    public class AppJsonWebToken
    {
        public string AccessToken { get; set; }
        public DateTime Expires { get; set; }
        public string UserId { get; set; }
        public string Email { get; set; }
        public bool? IsVerified { get; set; }
        public IList<string> Roles { get; set; }
        public IList<string> Permissions { get; set; }
    }
}