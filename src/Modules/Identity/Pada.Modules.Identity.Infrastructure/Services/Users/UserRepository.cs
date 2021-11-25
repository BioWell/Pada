using System;
using System.Linq;
using System.Threading.Tasks;
using EasyCaching.Core;
using Microsoft.AspNetCore.Identity;
using Pada.Infrastructure.Caching;
using Pada.Infrastructure.Types;
using Pada.Modules.Identity.Application.Users.Contracts;
using Pada.Modules.Identity.Application.Users.Dtos.GatewayResponses;
using Pada.Modules.Identity.Domain.Aggregates.Users;

namespace Pada.Modules.Identity.Infrastructure.Services.Users
{
    public class UserRepository : IUserRepository
    {
        private readonly CustomUserManager _userManager;
        private readonly IEasyCachingProvider _cachingProvider;

        public UserRepository(CustomUserManager userManager, 
            IEasyCachingProvider cachingProvider)
        {
            _userManager = userManager;
            _cachingProvider = cachingProvider;
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
            var appUser = await _userManager.FindByEmailAsync(email);
            return appUser?.ToUser();
        }
        
        public async Task<User> FindByNameAsync(string userName, bool invalidateCache = false)
        {
            var appUser = await _userManager.FindByNameAsync(userName);
            return appUser?.ToUser();
        }

        public async Task<CreateUserResponse> AddAsync(User user)
        {
            if (user is null)
                return new CreateUserResponse(new Error[]
                    { new("user_not_found", $"user can't be null") });

            var appUser = user.ToApplicationUser();

            IdentityResult identityResult;
            if (string.IsNullOrEmpty(user.Password))
                identityResult = await _userManager.CreateAsync(appUser);
            else
                identityResult = await _userManager.CreateAsync(appUser, user.Password);

            return new CreateUserResponse(Guid.Parse(appUser.Id), identityResult.Succeeded,
                identityResult.Errors.Select(e => new Error(e.Code, e.Description)));
        }
        
        public async Task<UpdateUserResponse> UpdateAsync(User user)
        {
            if (user is null)
                return new UpdateUserResponse(new Error[]
                    { new("user_not_found", $"user can't be null") });

            var appUser = user.ToApplicationUser();
            IdentityResult identityResult = await _userManager.UpdateAsync(appUser);

            return new UpdateUserResponse(appUser.ToUser(), identityResult.Succeeded,
                identityResult.Errors.Select(e => new Error(e.Code, e.Description)));
        }
        
        private async Task InvalidateCache(string key)
        {
            await _cachingProvider.RemoveAsync(key);
        }
    }
}