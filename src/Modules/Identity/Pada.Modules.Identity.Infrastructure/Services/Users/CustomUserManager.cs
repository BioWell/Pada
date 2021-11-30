using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using EasyCaching.Core;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Pada.Abstractions.Auth;
using Pada.Infrastructure.Caching;
using Pada.Infrastructure.Utils;
using Pada.Modules.Identity.Domain.Aggregates.Users;
using Pada.Modules.Identity.Infrastructure.Aggregates.Roles;
using Pada.Modules.Identity.Infrastructure.Aggregates.Users;

namespace Pada.Modules.Identity.Infrastructure.Services.Users
{
    public class CustomUserManager : AspNetUserManager<AppUser>
    {
        private readonly IEasyCachingProvider _cachingProvider;
        private readonly RoleManager<AppRole> _roleManager;
        private readonly IServiceScopeFactory _serviceScopeFactory;

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
            RoleManager<AppRole> roleManager,
            IServiceScopeFactory serviceScopeFactory)
            : base(store, optionsAccessor, passwordHasher, userValidators, passwordValidators, keyNormalizer, errors,
                services, logger)
        {
            _cachingProvider = cachingFactory.GetCachingProvider("mem");
            _roleManager = roleManager;
            _serviceScopeFactory = serviceScopeFactory;
        }

        public override async Task<AppUser> FindByLoginAsync(string loginProvider, string providerKey)
        {
            var cacheKey = CacheKey.With(nameof(FindByLoginAsync), loginProvider, providerKey);
            var result = await _cachingProvider.GetAsync(cacheKey, async () =>
            {
                var user = await base.FindByLoginAsync(loginProvider, providerKey);
                if (user != null)
                {
                    await LoadUserDetailsAsync(user);
                }

                return user;
            }, TimeSpan.FromMinutes(10));

            return result.Value!;
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

        public override async Task<AppUser> FindByNameAsync(string userName)
        {
            var cacheKey = CacheKey.With(nameof(FindByNameAsync), userName);
            var result = await _cachingProvider.GetAsync(cacheKey, async () =>
            {
                var user = await base.FindByNameAsync(userName);
                if (user != null)
                {
                    await LoadUserDetailsAsync(user);
                }

                return user;
            }, TimeSpan.FromMinutes(10));

            return result.Value!;
        }

        public override async Task<AppUser> FindByEmailAsync(string email)
        {
            var cacheKey = CacheKey.With(nameof(FindByEmailAsync), email);
            var result = await _cachingProvider.GetAsync(cacheKey, async () =>
            {
                var user = await base.FindByEmailAsync(email);
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

        public override async Task<IdentityResult> ResetPasswordAsync(AppUser user, string token, string newPassword)
        {
            //It is important to call base.FindByIdAsync method to avoid of update a cached user.
            var existUser = await base.FindByIdAsync(user.Id);
            var result = await base.ResetPasswordAsync(existUser, token, newPassword);

            return result;
        }

        public override async Task<IdentityResult> ChangePasswordAsync(AppUser user, string currentPassword,
            string newPassword)
        {
            var result = await base.ChangePasswordAsync(user, currentPassword, newPassword);
            if (result == IdentityResult.Success)
            {
                var cacheKey = CacheKey.With(GetType(), nameof(FindByIdAsync), user.Id);
                await _cachingProvider.RemoveAsync(cacheKey);
            }

            return result;
        }

        public override async Task<IdentityResult> DeleteAsync(AppUser user)
        {
            using var scope = _serviceScopeFactory.CreateScope();

            var result = await base.DeleteAsync(user);
            if (result.Succeeded)
            {
            }

            return result;
        }

        protected override async Task<IdentityResult> UpdateUserAsync(AppUser user)
        {
            var existentUser = await LoadExistingUser(user);

            //We cant update not existing user
            if (existentUser == null)
            {
                return IdentityResult.Failed(ErrorDescriber.DefaultError());
            }

            //We need to use Patch method to update already tracked by DbContent entity, unless the UpdateAsync for passed user will throw exception
            //"The instance of entity type 'ApplicationUser' cannot be tracked because another instance with the same key value for {'Id'} is already being tracked. When attaching existing entities, ensure that only one entity instance with a given key value is attached"
            user.Patch(existentUser);

            var result = await base.UpdateUserAsync(existentUser);

            return result;
        }

        public override async Task<IdentityResult> UpdateAsync(AppUser user)
        {
            var result = await base.UpdateAsync(user);
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

                await UpdateUserAsync(existentUser);
            }

            return result;
        }

        public override async Task<IdentityResult> CreateAsync(AppUser user)
        {
            var userResult = await base.CreateAsync(user);
            userResult.LogResult($"a new user with userId '{user.Id}' added successfully.");
            if (userResult.Succeeded)
            {
                var permissions = user.Permissions;
                var roles = user.Roles;
                if (permissions != null && permissions.Any())
                {
                    //Add
                    foreach (var permission in permissions)
                    {
                        var claimResult = await AddClaimAsync(user,
                            new Claim(CustomClaimTypes.Permission, permission.Name));
                        claimResult.LogResult($"claim {permission.Name} added to user '{user.Id}' successfully.");
                    }
                }

                if (roles != null && roles.Any())
                {
                    //Add
                    foreach (var newRole in roles)
                    {
                        if (await _roleManager.RoleExistsAsync(newRole.Name))
                        {
                            var roleResult = await AddToRoleAsync(user, newRole.Name);
                            roleResult.LogResult($"role '{newRole.Id}' added to user '{user.Id}' successfully.");
                        }
                    }
                }

                // add external logins
                if (!user.Logins.IsNullOrEmpty())
                {
                    foreach (var login in user.Logins)
                    {
                        await AddLoginAsync(user, new UserLoginInfo(login.LoginProvider, login.ProviderKey, null));
                    }
                }
            }

            return userResult;
        }

        public override async Task<IdentityResult> CreateAsync(AppUser user, string password)
        {
            var userResult = await base.CreateAsync(user, password);

            userResult.LogResult($"a new user with userId '{user.Id}' added successfully.",
                $"there is an exception in creating user with id '{user.Id}'");

            return userResult;
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