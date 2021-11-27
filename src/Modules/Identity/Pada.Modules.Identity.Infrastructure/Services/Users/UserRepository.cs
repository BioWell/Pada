using System;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using EasyCaching.Core;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Options;
using Pada.Infrastructure.Caching;
using Pada.Modules.Identity.Application.Users.Contracts;
using Pada.Modules.Identity.Application.Users.Dtos.GatewayResponses;
using Pada.Modules.Identity.Domain.Aggregates.Users;
using Pada.Modules.Identity.Infrastructure.Aggregates.Users;

namespace Pada.Modules.Identity.Infrastructure.Services.Users
{
    public class UserRepository : IUserRepository
    {
        private readonly CustomUserManager _userManager;
        private readonly IEasyCachingProvider _cachingProvider;
        private readonly IMapper _mapper;
        private readonly IdentityOptions _identityOptionsValue;

        public UserRepository(CustomUserManager userManager,
            IEasyCachingProvider cachingProvider,
            IMapper mapper, 
            IOptions<IdentityOptions> identityOptions)
        {
            _userManager = userManager;
            _cachingProvider = cachingProvider;
            _mapper = mapper;
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

            if (identityResult.Succeeded) 
                await InvalidateUserCache(appUser);
            
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

        private async Task InvalidateUserCache(AppUser appUser)
        {
            await InvalidateCache(CacheKey.With(nameof(FindByIdAsync), Guid.Parse(appUser.Id).ToString()));
            await InvalidateCache(CacheKey.With(nameof(FindByEmailAsync), appUser.Email));
            await InvalidateCache(CacheKey.With(nameof(FindByNameAsync), appUser.UserName));
        }
    }
}