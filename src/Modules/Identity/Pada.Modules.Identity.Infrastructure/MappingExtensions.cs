using System;
using System.Linq;
using Pada.Infrastructure.Utils;
using Pada.Modules.Identity.Domain.Aggregates.Users;
using Pada.Modules.Identity.Domain.Aggregates.Users.Types;
using Pada.Modules.Identity.Infrastructure.Aggregates.Roles;
using Pada.Modules.Identity.Infrastructure.Aggregates.Users;

namespace Pada.Modules.Identity.Infrastructure
{
    public static class MappingExtensions
    {
        public static User ToUser(this AppUser appUser)
        {
            if (appUser is null)
                return null;

            var userType = EnumUtility.SafeParse(appUser.UserType, UserType.Customer);
            var permissions = appUser.Permissions;
            var roles = appUser.Roles?.Select(x => Role.Of(x.Name, x.Name)).ToArray();
            var refreshTokens = appUser.RefreshTokens;
            var user = User.Create(new UserId(Guid.Parse(appUser.Id)),
                appUser.Email,
                appUser.FirstName,
                appUser.LastName,
                appUser.Name, 
                appUser.UserName, 
                appUser.PhoneNumber, 
                null!,
                userType, 
                appUser.LockoutEnabled,
                appUser.IsAdministrator, 
                appUser.IsActive, 
                appUser.EmailConfirmed,
                appUser.PhotoUrl, 
                appUser.Status,
                appUser.CreatedBy, 
                appUser.CreatedDate, 
                appUser.ModifiedBy,
                appUser.ModifiedDate);
            user.ChangePermissions(permissions,false);
            user.ChangeRoles(roles,false);
            user.ChangeRefreshTokens(refreshTokens?.ToArray());
            return user;
        }
        
        public static AppUser ToApplicationUser(this User user)
        {
            if (user is null)
                return null;

            var applicationUser = new AppUser
            {
                Name = user.Name,
                FirstName = user.FirstName,
                LastName = user.LastName,
                Email = user.Email,
                Id = user.Id.ToString(),
                Permissions = user.Permissions.ToList(),
                Roles = user.Roles.Select(x => x.ToApplicationRole()).ToList(),
                EmailConfirmed = user.EmailConfirmed,
                IsActive = user.IsActive,
                IsAdministrator = user.IsAdministrator,
                PhotoUrl = user.PhotoUrl,
                UserName = user.UserName,
                PhoneNumber = user.PhoneNumber,
                UserType = user.UserType.ToString(),
                CreatedBy = user.CreatedBy,
                CreatedDate = user.CreatedDate,
                ModifiedBy = user.ModifiedBy,
                ModifiedDate = user.ModifiedDate,
                SecurityStamp = Guid.NewGuid().ToString() 
            };

            applicationUser.Roles = user.Roles.Select(x => new AppRole()
            {
                Description = x.Description,
                Id = x.Code,
                Name = x.Name
            }).ToList();

            applicationUser.RefreshTokens = user.RefreshTokens?.ToList();

            return applicationUser;
        }
        
        public static AppRole ToApplicationRole(this Role role)
        {
            return new()
            {
                Description = role.Description, 
                Id = role.Name, 
                Name = role.Name, 
                Permissions = role.Permissions
            };
        }

        public static UserId ToUserId(this AppUser appUser)
        {
            if (appUser is null)
                return null;
            var userId = new UserId(Guid.Parse(appUser.Id),
                appUser.CreatedDate,
                appUser.ModifiedDate);
            return userId;
        }
    }
}