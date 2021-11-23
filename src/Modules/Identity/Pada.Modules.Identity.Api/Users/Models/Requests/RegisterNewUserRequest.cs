using System;
using System.Collections.Generic;
using Pada.Modules.Identity.Domain.Aggregates.Users.Types;

namespace Pada.Modules.Identity.Api.Users.Models.Requests
{
    public class RegisterNewUserRequest
    {
        public Guid? Id { get; set; }
        public string UserName { get; set; }
        public bool EmailConfirmed { get; set; }
        public string Email { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string PhoneNumber { get; set; }
        public string Name { get; set; }
        public bool IsAdministrator { get; set; }
        public string PhotoUrl { get; set; }
        public UserType UserType { get; set; }
        public string Status { get; set; }
        public string Password { get; set; }
        public bool LockoutEnabled { get; set; }
        public bool IsActive { get; set; }
        public IEnumerable<string> Roles { get; set; }
        public IEnumerable<string> Permissions { get; set; }
    }
}