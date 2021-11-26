using System;
using System.Collections.Generic;
using System.Linq;
using Pada.Abstractions.Auth;
using Pada.Abstractions.Domain.Types;
using Pada.Modules.Identity.Domain.Aggregates.Users.Types;

namespace Pada.Modules.Identity.Domain.Aggregates.Users
{
    public class User : AggregateRoot<Guid>
    {
        // Using a private collection field, better for DDD Aggregate's encapsulation
        private List<Role> _roles = new();
        private List<AppRefreshToken> _refreshTokens = new();
        private List<AppPermission> _permissions = new();
        public string UserName { get; private set; }
        public bool EmailConfirmed { get; private set; }
        public string Email { get; private set; }
        public string FirstName { get; private set; }
        public string LastName { get; private set; }
        public string Name { get; private set; }
        public bool IsAdministrator { get; private set; }
        public string PhotoUrl { get; private set; }
        public UserType UserType { get; private set; }
        public string Status { get; private set; }
        public string Password { get; private set; }
        public string PhoneNumber { get; private set; }
        public bool IsActive { get; private set; }
        public bool PasswordExpired { get; private set; }
        public DateTime? LastPasswordChangedDate { get; private set; }
        public DateTime CreatedDate { get; private set; }
        public DateTime ModifiedDate { get; private set; }
        public string CreatedBy { get; private set; }
        public string ModifiedBy { get; private set; }
        public IReadOnlyList<Role> Roles => _roles.AsReadOnly();
        public IReadOnlyList<AppPermission> Permissions => _permissions.AsReadOnly();
        public IReadOnlyList<AppRefreshToken> RefreshTokens => _refreshTokens.AsReadOnly();

        private User(UserId id,
            string password,
            bool isAdmin = false,
            bool isActive = true,
            bool emailConfirmed = false,
            string createdBy = null,
            DateTime? createdDate = null,
            string modifiedBy = null,
            DateTime? modifiedDate = null)
        {
            Id = id;
            Password = password;
            EmailConfirmed = emailConfirmed;
            IsAdministrator = isAdmin;
            IsActive = isActive;
            CreatedDate = createdDate ?? DateTime.Now;
            CreatedBy = createdBy;
            ModifiedBy = modifiedBy;
            ModifiedDate = modifiedDate?? DateTime.Now;;
        }

        private User()
        {
            // Only for deserialization 
        }
        
        public static User Create(UserId id,
            string email,
            string firstName,
            string lastName,
            string name,
            string userName,
            string phoneNumber,
            string password,
            UserType userType,
            bool isAdmin = false,
            bool isActive = true,
            bool emailConfirmed = false,
            string photoUrl = null,
            string status = null,
            string createdBy = null,
            DateTime? createdDate = null,
            string modifiedBy = null,
            DateTime? modifiedDate = null)
        {
            var user = new User(id,
                password,
                isAdmin,
                isActive,
                emailConfirmed,
                createdBy,
                createdDate,
                modifiedBy,
                modifiedDate)
            {
            };
            
            user.SetPersonalInformation(firstName, lastName, name.Trim(), email?.ToLowerInvariant(), phoneNumber,
                photoUrl);
            user.SetUserName(userName);
            user.SetStatus(status);
            user.SetUserType(userType);
            return user;
        }
        
        private void SetPersonalInformation(string firstName, string lastName,
            string name, string email, string phoneNumber, string photoUrl)
        {
            FirstName = firstName;
            LastName = lastName;
            Name = name;
            PhoneNumber = phoneNumber;
            PhotoUrl = photoUrl;
            Email = email;
        }
        
        private void SetUserName(string userName)
        {
            UserName = userName;
        }
        
        private void SetStatus(string status)
        {
            if (string.IsNullOrEmpty(status))
                return;
            Status = status;
        }
        
        private void SetUserType(UserType userType)
        {
            UserType = userType;
        }
        
        public void ChangePermissions(IList<AppPermission> permissions)
        {
            if (permissions is null)
                return;
            _permissions = permissions.ToList();
        }
        
        public void ChangeRoles(IList<Role> roles)
        {
            if (roles is null)
                return;
            _roles = roles.ToList();
        }
        
        public void ChangeRefreshTokens(IList<AppRefreshToken> refreshTokens)
        {
            if (refreshTokens is null)
                return;
            _refreshTokens = refreshTokens.ToList();
        }
        
        public void ActivateUser()
        {
            IsActive = true;
        }
        
        public void DeactivateUser()
        {
            IsActive = false;
        }
    }
}