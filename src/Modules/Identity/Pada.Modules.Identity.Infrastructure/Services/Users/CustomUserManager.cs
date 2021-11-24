using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using EasyCaching.Core;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
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
            : base(store, optionsAccessor, passwordHasher, userValidators, passwordValidators, keyNormalizer, errors, services, logger)
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
    }
}