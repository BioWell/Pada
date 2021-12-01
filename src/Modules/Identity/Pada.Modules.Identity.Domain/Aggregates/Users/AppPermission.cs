using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using Pada.Abstractions.Auth;
using Pada.Abstractions.Domain.Types;
using Pada.Infrastructure.Utils;

namespace Pada.Modules.Identity.Domain.Aggregates.Users
{
    public sealed class AppPermission : ValueObject
    {
        private const char ScopeCharSeparator = '|';

        public string Name { get; private set; }
        public string GroupName { get; private set; }

        public static class Roles
        {
            public static readonly AppPermission View = new(SecurityConstants.Permission.Roles.View, "role");
            public static readonly AppPermission Create = new(SecurityConstants.Permission.Roles.Create, "role");
            public static readonly AppPermission Edit = new(SecurityConstants.Permission.Roles.Edit, "role");
            public static readonly AppPermission Delete = new(SecurityConstants.Permission.Roles.Delete, "role");
            public static readonly AppPermission Search = new(SecurityConstants.Permission.Roles.Search, "role");

            public static AppPermission[] Permissions
            {
                get
                {
                    return new[]
                    {
                        Create,
                        Delete,
                        Edit,
                        Search,
                        View,
                    };
                }
            }
        }

        public static class Security
        {
            public static readonly AppPermission VerifyEmail = new(SecurityConstants.Permission.Security.VerifyEmail,
                "security");

            public static readonly AppPermission ApiAccess = new(SecurityConstants.Permission.Security.ApiAccess,
                "security");

            public static AppPermission[] Permissions
            {
                get
                {
                    return new[]
                    {
                        VerifyEmail,
                        ApiAccess
                    };
                }
            }
        }

        public static class Users
        {
            public static readonly AppPermission View = new(SecurityConstants.Permission.Users.View, "user");
            public static readonly AppPermission Create = new(SecurityConstants.Permission.Users.Create, "user");
            public static readonly AppPermission Edit = new(SecurityConstants.Permission.Users.Edit, "user");
            public static readonly AppPermission Delete = new(SecurityConstants.Permission.Users.Delete, "user");
            public static readonly AppPermission Search = new(SecurityConstants.Permission.Users.Search, "user");
            public static readonly AppPermission Export = new(SecurityConstants.Permission.Users.Export, "user");

            public static AppPermission[] Permissions
            {
                get
                {
                    return new[]
                    {
                        Create,
                        Delete,
                        Edit,
                        Search,
                        View,
                        Export
                    };
                }
            }
        }

        public AppPermission(string name, string groupName = null)
        {
            Name = name;
            GroupName = groupName;
        }

        public static AppPermission Of(string name, string groupName = null)
        {
            return new(name, groupName);
        }

        public static AppPermission[] GetAllPermissions()
        {
            return _ = AppPermission.Roles.Permissions
                .Union(AppPermission.Users.Permissions)
                .Union(AppPermission.Security.Permissions)
                .ToArray();
        }

        protected override IEnumerable<object> GetEqualityComponents()
        {
            yield return Name;
        }

        public void Patch(AppPermission target)
        {
            target.Name = Name;
            target.GroupName = GroupName;
        }

        public static AppPermission TryCreateFromClaim(Claim claim)
        {
            AppPermission result = null!;
            if (claim.Type.EqualsInvariant(CustomClaimTypes.Permission))
            {
                result = new(claim.Value);
                if (result.Name.Contains(ScopeCharSeparator))
                {
                    var parts = claim.Value.Split(ScopeCharSeparator);
                    result.Name = parts.First();
                }
            }

            return result;
        }
    }
}