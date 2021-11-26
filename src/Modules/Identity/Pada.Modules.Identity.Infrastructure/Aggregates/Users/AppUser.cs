using System;
using System.Collections.Generic;
using Microsoft.AspNetCore.Identity;
using Pada.Abstractions.Auth;
using Pada.Abstractions.Domain.Types;
using Pada.Modules.Identity.Domain.Aggregates.Users;
using Pada.Modules.Identity.Infrastructure.Aggregates.Roles;

namespace Pada.Modules.Identity.Infrastructure.Aggregates.Users
{
    public class AppUser : IdentityUser<string>, IAuditable
    {
        public bool IsActive { get; set; }
        public bool IsAdministrator { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Name { get; set; }
        public string PhotoUrl { get; set; }
        public string UserType { get; set; }
        public string Status { get; set; }
        public AccountState UserState { get; set; }
        public DateTime CreatedDate { get; set; }
        public DateTime ModifiedDate { get; set; }
        public string CreatedBy { get; set; }
        public string ModifiedBy { get; set; }
        public bool PasswordExpired { get; set; }
        public string Password { get; set; }

        public DateTime? LastPasswordChangedDate { get; set; }
        public IList<AppPermission> Permissions { get; set; }
        public IList<AppRole> Roles { get; set; } 

        public virtual ICollection<AppUserRole> UserRoles { get; set; }
        public virtual ICollection<IdentityUserClaim<string>> Claims { get; set; }
        public virtual ICollection<IdentityUserLogin<string>> Logins { get; set; }
        public virtual ICollection<IdentityUserToken<string>> Tokens { get; set; }
        public virtual ICollection<AppRefreshToken> RefreshTokens { get; set; }

        public virtual void Patch(AppUser target)
        {
            target.UserName = UserName;
            target.IsAdministrator = IsAdministrator;
            target.IsActive = IsActive;
            target.Email = Email;
            target.FirstName = FirstName;
            target.LastName = LastName;
            target.Name = Name;
            target.NormalizedEmail = NormalizedEmail;
            target.NormalizedUserName = NormalizedUserName;
            target.EmailConfirmed = EmailConfirmed;
            target.SecurityStamp = SecurityStamp;
            target.PhoneNumberConfirmed = PhoneNumberConfirmed;
            target.PhoneNumber = PhoneNumber;
            target.TwoFactorEnabled = TwoFactorEnabled;
            target.LockoutEnabled = LockoutEnabled;
            target.LockoutEnd = LockoutEnd;
            target.AccessFailedCount = AccessFailedCount;
            target.PhotoUrl = PhotoUrl;
            target.UserType = UserType;
            target.Status = Status;
            target.Roles = Roles;
            target.Permissions = Permissions;
        }
    }
}