using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using EasyCaching.Core;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Pada.Abstractions.Auth;
using Pada.Infrastructure.Caching;
using Pada.Modules.Identity.Domain.Aggregates.Users;
using Pada.Modules.Identity.Infrastructure.Aggregates.Roles;
using Pada.Modules.Identity.Infrastructure.Aggregates.Users;

namespace Pada.Modules.Identity.Infrastructure.Services.Users
{
    public class CustomUserManager : AspNetUserManager<AppUser>
    {
        private readonly IEasyCachingProvider _cachingProvider;
        private readonly RoleManager<AppRole> _roleManager;

        public CustomUserManager(IUserStore<AppUser> store,
            IOptions<IdentityOptions> optionsAccessor,
            IPasswordHasher<AppUser> passwordHasher,
            IEnumerable<IUserValidator<AppUser>> userValidators,
            IEnumerable<IPasswordValidator<AppUser>> passwordValidators,
            ILookupNormalizer keyNormalizer,
            IdentityErrorDescriber errors,
            IServiceProvider services,
            ILogger<CustomUserManager> logger,
            IEasyCachingProviderFactory cachingFactory,
            RoleManager<AppRole> roleManager)
            : base(store, optionsAccessor, passwordHasher, userValidators, passwordValidators, keyNormalizer, errors,
                services, logger)
        {
            _cachingProvider = cachingFactory.GetCachingProvider("mem");
            _roleManager = roleManager;
        }

        public override async Task<AppUser> FindByIdAsync(string userId)
        {
            var cacheKey = CacheKey.With(nameof(FindByIdAsync), userId);
            var result = await _cachingProvider.GetAsync(cacheKey, async () =>
            {
                var user = await base.FindByIdAsync(userId);
                if (user != null)
                {
                    await LoadUserDetailsAsync(user);
                }

                return user;
            }, TimeSpan.FromMinutes(10));

            return result.Value!;
        }

        protected virtual async Task LoadUserDetailsAsync(AppUser user)
        {
            if (user == null)
            {
                throw new ArgumentNullException(nameof(user));
            }

            await LoadRelatedRoles(user);
            await LoadRelatedPermissions(user);
            await LoadRelatedRefreshTokens(user);
            await LoadRelatedLogins(user);
        }

        private async Task LoadRelatedRoles(AppUser user)
        {
            if (user is not null && user.Roles is null)
            {
                user.Roles = new List<AppRole>();
                foreach (var roleName in await base.GetRolesAsync(user))
                {
                    var role = await _roleManager.FindByNameAsync(roleName);
                    if (role != null)
                    {
                        user.Roles.Add(role);
                    }
                }
            }
        }

        private async Task LoadRelatedPermissions(AppUser user)
        {
            if (user is not null && user.Permissions is null)
            {
                user.Permissions = (await GetClaimsAsync(user)).Select(AppPermission.TryCreateFromClaim).ToList();
            }
        }

        private async Task LoadRelatedLogins(AppUser user)
        {
            if (user is not null && user.Logins is null)
            {
                // Read associated logins (compatibility with v2)
                var logins = await base.GetLoginsAsync(user);
                user.Logins = logins.Select(x => new IdentityUserLogin<string>
                {
                    LoginProvider = x.LoginProvider, ProviderKey = x.ProviderKey
                }).ToArray();
            }
        }

        public async Task LoadRelatedRefreshTokens(AppUser user)
        {
            if (user is not null && user.RefreshTokens is null)
            {
                var query = Users.Include(x => x.RefreshTokens);
                var refreshTokens = (await query.SingleOrDefaultAsync(x => x.UserName == user.UserName));
                user.RefreshTokens = null;
            }
        }

        public override async Task<IdentityResult> UpdateAsync(AppUser user)
        {
            var existentUser = await LoadExistingUser(user);

            var permissions = user.Permissions;
            var roles = user.Roles;
            var userLogins = existentUser.Logins.Select(x => new UserLoginInfo(x.LoginProvider, x.ProviderKey,
                null)).ToList();
            var refreshTokens = user.RefreshTokens;

            if (roles != null)
            {
                var targetRoles = await GetRolesAsync(existentUser);
                var sourceRoles = roles.Select(x => x.Name).ToList();
                //Add
                foreach (var newRole in sourceRoles.Except(targetRoles))
                {
                    await AddToRoleAsync(existentUser, newRole);
                }

                //Remove
                foreach (var removeRole in targetRoles.Except(sourceRoles))
                {
                    await RemoveFromRoleAsync(existentUser, removeRole);
                }
            }

            if (permissions != null)
            {
                var targetPermissions = (await GetClaimsAsync(existentUser)).Select(x => x.Value).ToList();
                var sourcePermissions = permissions.Select(x => x.Name).ToList();
                //Add
                foreach (var newPermission in sourcePermissions.Except(targetPermissions))
                {
                    await AddClaimAsync(existentUser,
                        new Claim(CustomClaimTypes.Permission, newPermission));
                }

                //Remove
                foreach (var removePermission in targetPermissions.Except(sourcePermissions))
                {
                    await RemoveClaimAsync(existentUser,
                        new Claim(CustomClaimTypes.Permission, removePermission));
                }
            }

            if (user.Logins != null)
            {
                var targetLogins = await GetLoginsAsync(existentUser);
                var sourceLogins = userLogins;
                //Add
                foreach (var item in sourceLogins.Where(x =>
                    targetLogins.All(y => x.LoginProvider + x.ProviderKey != y.LoginProvider + y.ProviderKey)))
                {
                    await AddLoginAsync(existentUser, item);
                }

                //Remove
                foreach (var item in targetLogins.Where(x =>
                    sourceLogins.All(y => x.LoginProvider + x.ProviderKey != y.LoginProvider + y.ProviderKey)))
                {
                    await RemoveLoginAsync(existentUser, item.LoginProvider, item.ProviderKey);
                }
            }

            if (user.RefreshTokens != null)
            {
                var targetTokens = existentUser.RefreshTokens;
                var sourceTokens = refreshTokens;

                //Add
                foreach (var token in sourceTokens.Except(targetTokens))
                {
                    existentUser.RefreshTokens.Add(token);
                }

                //Remove
                foreach (var removeToken in targetTokens.Except(sourceTokens))
                {
                    existentUser.RefreshTokens.Remove(removeToken);
                }
            }

            var result = await base.UpdateAsync(existentUser);

            return result;
        }

        protected virtual async Task<AppUser> LoadExistingUser(AppUser user)
        {
            AppUser result = null;

            if (!string.IsNullOrEmpty(user.Id))
            {
                //It is important to call base.FindByIdAsync method to avoid of update a cached user.
                result = await base.FindByIdAsync(user.Id);
            }

            if (result == null)
            {
                //It is important to call base.FindByNameAsync method to avoid of update a cached user.
                result = await base.FindByNameAsync(user.UserName);
            }

            if (result != null)
            {
                await LoadUserDetailsAsync(result);
            }

            return result;
        }
    }
}