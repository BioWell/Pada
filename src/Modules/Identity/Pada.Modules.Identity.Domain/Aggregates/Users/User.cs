using System;
using System.Collections.Generic;
using System.Linq;
using Ardalis.GuardClauses;
using Pada.Abstractions.Auth;
using Pada.Infrastructure.Domain;
using Pada.Infrastructure.Utils;
using Pada.Modules.Identity.Domain.Aggregates.Users.DomainEvents;
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
        public bool LockoutEnabled { get; private set; }
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
            bool isLockoutEnabled,
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
            LockoutEnabled = isLockoutEnabled;
            EmailConfirmed = emailConfirmed;
            IsAdministrator = isAdmin;
            IsActive = isActive;
            CreatedDate = createdDate ?? DateTime.Now;
            CreatedBy = createdBy;
            ModifiedBy = modifiedBy;
            ModifiedDate = modifiedDate ?? DateTime.Now;
            ;
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
            bool isLockoutEnabled,
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
                isLockoutEnabled,
                isAdmin,
                isActive,
                emailConfirmed,
                createdBy,
                createdDate,
                modifiedBy,
                modifiedDate)
            {
                Version = 0
            };

            user.SetPersonalInformation(firstName, lastName, name.Trim(), email?.ToLowerInvariant(), phoneNumber,
                photoUrl, false);
            user.SetUserName(userName);
            user.SetStatus(status);
            user.SetUserType(userType);
            user.AddDomainEvent(new NewUserRegisteredDomainEvent(user));
            return user;
        }

        public void ChangeStatus(string status)
        {
            SetStatus(status);
            IncrementVersion();
            AddDomainEvent(new UserStatusChangedDomainEvent(Id, status));
        }

        private void SetStatus(string status)
        {
            if (string.IsNullOrEmpty(status))
                return;
            Status = status;
        }

        public void ChangeUserType(UserType userType)
        {
            SetUserType(userType);
            IncrementVersion();
            AddDomainEvent(new UserTypeChangedDomainEvent(Id, userType));
        }

        private void SetUserType(UserType userType)
        {
            UserType = userType;
        }

        public void MarkAsAdmin()
        {
            IsAdministrator = true;
            IncrementVersion();
            AddDomainEvent(new UserMarkedAsAdminDomainEvent(Id));
        }

        public void MarkAsUser()
        {
            IsAdministrator = false;
            IncrementVersion();
            AddDomainEvent(new UserMarkedAsUserDomainEvent(Id));
        }

        public void ActivateUser()
        {
            IsActive = true;
            IncrementVersion();
            AddDomainEvent(new UserActivatedDomainEvent(Id));
        }

        public void DeactivateUser()
        {
            IsActive = false;
            IncrementVersion();
            AddDomainEvent(new UserDeactivateDomainEvent(Id));
        }

        public void ChangeUserName(string userName)
        {
            SetUserName(userName);
            IncrementVersion();
            AddDomainEvent(new UserNameChangedDomainEvent(Id, userName));
        }

        private void SetUserName(string userName)
        {
            // we validate username in this domain event immediately
            // DomainEvents.Raise(new ChangingUserNameDomainEvent(userName));
            UserName = userName;
        }

        public void ChangePhotoUrl(string photoUrl)
        {
            Guard.Against.NullOrEmpty(photoUrl, nameof(PhotoUrl));
            PhotoUrl = photoUrl;
            IncrementVersion();
            AddDomainEvent(new PhotoUrlChangedDomainEvent(Id, photoUrl));
        }

        public void ChangePersonalInformation(string firstName, string lastName,
            string name, string email, string phoneNumber, string photoUrl)
        {
            SetPersonalInformation(firstName, lastName, name, email, phoneNumber, photoUrl, true);
            IncrementVersion();
            AddDomainEvent(new PersonalInformationChangedDomainEvent(this));
        }

        private void SetPersonalInformation(string firstName, string lastName,
            string name, string email, string phoneNumber, string photoUrl, bool addDomain)
        {
            FirstName = firstName;
            LastName = lastName;
            Name = name;
            PhoneNumber = phoneNumber;
            PhotoUrl = photoUrl;
            Email = email;

            if (addDomain)
            {
                IncrementVersion();
                AddDomainEvent(new PersonalInformationChangedDomainEvent(this));
            }
        }

        #region Domain Operations

        public void ChangeRoles(IList<Role> roles, bool addDomain)
        {
            if (roles is null)
                return;
            _roles = roles.ToList();
            if (addDomain)
            {
                IncrementVersion();
                AddDomainEvent(new RolesChangedDomainEvent(Id, _roles));
            }
        }

        public void ChangePermissions(IList<AppPermission> permissions, bool addDomain)
        {
            if (permissions is null)
                return;
            _permissions = permissions.ToList();

            if (addDomain)
            {
                IncrementVersion();
                AddDomainEvent(new PermissionChangedDomainEvent(Id, _permissions));
            }
        }

        public void ChangeRefreshTokens(IList<AppRefreshToken> refreshTokens)
        {
            if (refreshTokens is null)
                return;
            _refreshTokens = refreshTokens.ToList();
            IncrementVersion();
            AddDomainEvent(new RefreshTokenChangedDomainEvent(Id, _refreshTokens));
        }

        public bool HasValidRefreshToken(string token)
        {
            return _refreshTokens.Any(refreshToken =>
                refreshToken.Token == token && IsRefreshTokenValid(refreshToken));
        }

        public bool IsRefreshTokenValid(AppRefreshToken existingToken, double? ttlRefreshToken = null)
        {
            // Token already expired or revoked, then return false
            if (existingToken.IsActive == false)
            {
                return false;
            }

            if (ttlRefreshToken is not null && existingToken.CreatedOn.AddDays((long) ttlRefreshToken) <= DateTime.Now)
            {
                return false;
            }

            return true;
        }

        public void RemoveOldRefreshTokens(long? ttlRefreshToken = null)
        {
            _refreshTokens.RemoveAll(x => IsRefreshTokenValid(x, ttlRefreshToken) == false);
        }

        public void RevokeRefreshToken(AppRefreshToken refreshToken, string ip = null)
        {
            refreshToken.RevokedOn = DateTime.Now;
            refreshToken.RevokedByIp = ip ?? IpHelper.GetIpAddress();
            AddDomainEvent(new RefreshTokenRevokedDomainEvent(Id, refreshToken));
            IncrementVersion();
        }

        public void RevokeDescendantRefreshTokens(AppRefreshToken refreshToken, string ip = null)
        {
            // recursively traverse the refresh token chain and ensure all descendants are revoked
            if (!string.IsNullOrEmpty(refreshToken.Token))
            {
                var childToken = _refreshTokens.SingleOrDefault(x => x.Token == refreshToken.Token);
                if (childToken == null)
                    return;

                if (childToken.IsActive)
                    RevokeRefreshToken(childToken, ip ?? IpHelper.GetIpAddress());
                else
                    RevokeDescendantRefreshTokens(childToken, ip ?? IpHelper.GetIpAddress());
            }
        }

        public void RemoveRefreshToken(string refreshToken)
        {
            _refreshTokens.Remove(_refreshTokens.First(t => t.Token == refreshToken));
        }

        #endregion
    }
}