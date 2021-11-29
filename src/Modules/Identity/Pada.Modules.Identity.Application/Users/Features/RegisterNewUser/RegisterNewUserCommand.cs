using System;
using System.Collections.Generic;
using MediatR;
using Pada.Modules.Identity.Domain.Aggregates.Users.Types;

namespace Pada.Modules.Identity.Application.Users.Features.RegisterNewUser
{
    public class RegisterNewUserCommand : IRequest
    {
        public string UserName { get; }
        public bool EmailConfirmed { get; }
        public string Email { get; }
        public string FirstName { get; }
        public string LastName { get; }
        public string PhoneNumber { get; }
        public string Name { get; }
        public bool IsAdministrator { get; }
        public string PhotoUrl { get; }
        public UserType UserType { get; }
        public string Status { get; }
        public string Password { get; }
        public bool IsActive { get; }
        public IEnumerable<string> Roles { get; }
        public IEnumerable<string> Permissions { get; }
        public Guid Id { get; set; }
        public Guid CorrelationId { get; set; }
        public DateTime OccurredOn { get; set; } = DateTime.Now;

        public RegisterNewUserCommand(Guid id, string email, string firstName, string lastName,
            string name, string userName, string phoneNumber, string password,
            IReadOnlyList<string> permissions, UserType userType, bool isAdmin = false, bool isActive = true,
            IReadOnlyList<string> roles = null, bool emailConfirmed = false,
            string photoUrl = null, string status = null)
        {
            Id = id;
            UserName = userName;
            IsActive = true;
            Email = email?.ToLowerInvariant();
            FirstName = firstName;
            LastName = lastName;
            Name = name?.Trim();
            UserType = userType;
            Password = password;
            PhoneNumber = phoneNumber;
            Roles = roles;
            Permissions = permissions;
            EmailConfirmed = emailConfirmed;
            PhotoUrl = photoUrl;
            Status = status;
            IsAdministrator = isAdmin;
            IsActive = isActive;
        }
    }
}