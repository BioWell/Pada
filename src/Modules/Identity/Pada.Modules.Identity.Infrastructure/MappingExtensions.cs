using System;
using System.Linq;
using Pada.Infrastructure.Utils;
using Pada.Modules.Identity.Domain.Aggregates.Users;
using Pada.Modules.Identity.Domain.Aggregates.Users.Types;
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

            var user = User.Create(new UserId(Guid.Parse(appUser.Id)), appUser.Email, appUser.FirstName,
                appUser.LastName,
                appUser.Name, appUser.UserName, appUser.PhoneNumber, null!,
                userType, appUser.IsAdministrator, appUser.IsActive, appUser.EmailConfirmed,
                appUser.PhotoUrl, appUser.Status,appUser.CreatedBy, appUser.CreatedDate, appUser.ModifiedBy,
                appUser.ModifiedDate);

            return user;
        }
    }
}