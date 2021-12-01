using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using AutoMapper;
using EasyCaching.Core;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Pada.Abstractions.Auth;
using Pada.Infrastructure.Caching;
using Pada.Modules.Identity.Application.Users.Contracts;
using Pada.Modules.Identity.Application.Users.Dtos;
using Pada.Modules.Identity.Domain.Aggregates.Users;
using Pada.Modules.Identity.Infrastructure.Persistence;

namespace Pada.Modules.Identity.Infrastructure.Services.Users
{
    public class UserRepository : IUserRepository
    {
        private string UserPrefix = "pada:user:";

        private readonly CustomUserManager _userManager;
        private readonly AppIdentityDbContext _dbContext;
        private readonly IEasyCachingProvider _cachingProvider;
        private readonly IMapper _mapper;
        private readonly IdentityOptions _identityOptionsValue;

        public UserRepository(CustomUserManager userManager,
            IEasyCachingProvider cachingProvider,
            IMapper mapper,
            IOptions<IdentityOptions> identityOptions,
            AppIdentityDbContext dbContext)
        {
            _userManager = userManager;
            _cachingProvider = cachingProvider;
            _mapper = mapper;
            _dbContext = dbContext;
            _identityOptionsValue = identityOptions.Value;
        }

        public async Task<User> FindByIdAsync(string id, bool invalidateCache = false)
        {
            if (invalidateCache)
                await InvalidateCache(CacheKey.With(nameof(FindByIdAsync), id));
            var appUser = await _userManager.FindByIdAsync(id);
            return appUser?.ToUser();
        }

        public async Task<User> FindByEmailAsync(string email, bool invalidateCache = false)
        {
            if (invalidateCache)
                await InvalidateCache(CacheKey.With(nameof(FindByEmailAsync), email));
            var appUser = await _userManager.FindByEmailAsync(email);
            return appUser?.ToUser();
        }

        public async Task<User> FindByNameAsync(string userName, bool invalidateCache = false)
        {
            if (invalidateCache)
                await InvalidateCache(CacheKey.With(nameof(FindByNameAsync), userName));
            var appUser = await _userManager.FindByNameAsync(userName);
            return appUser?.ToUser();
        }

        public async Task<User> FindByPhoneAsync(string phone, bool invalidateCache = false)
        {
            if (invalidateCache)
                await InvalidateCache(CacheKey.With(nameof(FindByPhoneAsync), phone));
            var appUser = _dbContext.Users.FirstOrDefault(c => c.PhoneNumber == phone);
            return appUser?.ToUser();
        }

        public async Task<User> FindByNameOrEmailAsync(string userNameOrEmail, bool invalidateCache = false)
        {
            if (invalidateCache) await InvalidateCache(CacheKey.With(nameof(FindByNameOrEmailAsync), userNameOrEmail));

            var appUser = await _userManager.FindByNameAsync(userNameOrEmail) ??
                          await _userManager.FindByEmailAsync(userNameOrEmail);

            return appUser?.ToUser();
        }

        public async Task<User> FindByRefreshToken(string refreshToken)
        {
            var appUser = await _userManager.Users.Include(x => x.RefreshTokens)
                .SingleOrDefaultAsync(u => u.RefreshTokens.Any(r => r.Token == refreshToken));

            return appUser?.ToUser();
        }

        public async Task<(User, bool)> IsUserLockedAsync(string userNameOrEmail)
        {
            var appUser = await _userManager.FindByNameAsync(userNameOrEmail) ??
                          await _userManager.FindByEmailAsync(userNameOrEmail);

            if (appUser is null) return (null, false);

            var isLocked = await _userManager.IsLockedOutAsync(appUser);

            return (appUser?.ToUser(), isLocked);
        }

        public async Task<CheckPasswordResponse> CheckPassword(string userName, string password)
        {
            var appUser = await _userManager.FindByNameAsync(userName);
            if (appUser is null)
                return new CheckPasswordResponse("user_not_found", $"User not found for userName: `{userName}`");

            return new CheckPasswordResponse(await _userManager.CheckPasswordAsync(appUser, password));
        }

        public async Task<(IList<Claim> UserClaims, IList<string> Roles, IList<string> PermissionClaims)>
            GetClaimsAsync(string userName)
        {
            var appUser = await _userManager.FindByNameAsync(userName);
            var userClaims =
                (await _userManager.GetClaimsAsync(appUser))
                .Where(x => x.Type != CustomClaimTypes.Permission)
                .ToList();
            var roles = (await _userManager.GetRolesAsync(appUser));
            var permissions = (await _userManager.GetClaimsAsync(appUser))
                .Where(x => x.Type == CustomClaimTypes.Permission)?
                .Select(x => x.Value)
                .ToList();

            return (UserClaims: userClaims, Roles: roles, PermissionClaims: permissions);
        }

        public bool IsPhoneUsedAsync(string phone)
        {
            return _userManager.Users.Any(c => c.PhoneNumber == phone);
        }

        public async Task<CreateUserResponse> AddAsync(User user)
        {
            if (user is null)
                return new CreateUserResponse("user_not_found", $"user can't be null");

            var appUser = user.ToApplicationUser();

            IdentityResult identityResult;
            if (string.IsNullOrEmpty(user.Password))
                identityResult = await _userManager.CreateAsync(appUser);
            else
                identityResult = await _userManager.CreateAsync(appUser, user.Password);

            return new CreateUserResponse(Guid.Parse(appUser.Id),
                identityResult.Succeeded,
                identityResult.Errors
                    .Distinct()
                    .ToDictionary(x => x.Code, x => new string[] {x.Description}));
        }

        public async Task<UpdateUserResponse> UpdateAsync(User user)
        {
            if (user is null)
                return new UpdateUserResponse("user_not_found", $"user can't be null");

            var appUser = user.ToApplicationUser();
            IdentityResult identityResult = await _userManager.UpdateAsync(appUser);

            // if (identityResult.Succeeded)
            //     await InvalidateUserCache(appUser);

            return new UpdateUserResponse(appUser.ToUserId(),
                identityResult.Succeeded,
                identityResult.Errors
                    .Distinct()
                    .ToDictionary(x => x.Code, x => new string[] {x.Description}));
        }

        public async Task<LockUserResponse> LockUserAsync(string userId)
        {
            var appUser = await _userManager.FindByIdAsync(userId);

            if (appUser is null)
                return new LockUserResponse("user_not_found", $"User not found for userId: `{userId}`");

            if (appUser.LockoutEnabled == false)
                return new LockUserResponse("LockoutEnabled", $"LockoutEnabled is : `{appUser.LockoutEnabled}`");

            var duration = _identityOptionsValue?.Lockout?.DefaultLockoutTimeSpan ?? TimeSpan.FromMinutes(15);

            var identityResult = await _userManager.SetLockoutEndDateAsync(appUser,
                DateTimeOffset.Now + duration);

            return new LockUserResponse(appUser.ToUserId(),
                identityResult.Succeeded,
                identityResult.Errors
                    .Distinct()
                    .ToDictionary(x => x.Code, x => new string[] {x.Description}));
        }

        private async Task InvalidateCache(string key)
        {
            await _cachingProvider.RemoveAsync(key);
        }
    }
}