using System;

namespace Pada.Modules.Identity.Api.Users.Models.Requests
{
    public class ChangePersonalInfoRequest
    {
        public Guid? Id { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Name { get; set; }
        public string Email { get; set; }
        public string PhoneNumber { get; set; }
        public string PhotoUrl { get; set; }
    }
}