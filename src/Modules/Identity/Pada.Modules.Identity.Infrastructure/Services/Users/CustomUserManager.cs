using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Pada.Modules.Identity.Infrastructure.Aggregates.Users;

namespace Pada.Modules.Identity.Infrastructure.Services.Users
{
    public class CustomUserManager : AspNetUserManager<AppUser>
    {
        public CustomUserManager(IUserStore<AppUser> store, 
            IOptions<IdentityOptions> optionsAccessor, 
            IPasswordHasher<AppUser> passwordHasher, 
            IEnumerable<IUserValidator<AppUser>> userValidators, 
            IEnumerable<IPasswordValidator<AppUser>> passwordValidators, 
            ILookupNormalizer keyNormalizer, 
            IdentityErrorDescriber errors, 
            IServiceProvider services, 
            ILogger<CustomUserManager> logger) 
            : base(store, optionsAccessor, passwordHasher, userValidators, passwordValidators, keyNormalizer, errors, services, logger)
        {
        }
        
        // public override async Task<AppUser> FindByEmailAsync(string email)
        // {
        //     var user = await base.FindByEmailAsync(email);
        //     if (user != null)
        //     {
        //         await LoadUserDetailsAsync(user);
        //     }
        //
        //     return user;
        // }
        //
        // public override async Task<AppUser> FindByNameAsync(string userName)
        // {
        //     var user = await base.FindByNameAsync(userName);
        //     if (user != null)
        //     {
        //         await LoadUserDetailsAsync(user);
        //     }
        //
        //     return user;
        // }
        //
        // public override async Task<AppUser> FindByIdAsync(string userId)
        // {
        //     var user = await base.FindByIdAsync(userId);
        //     if (user != null)
        //     {
        //         await LoadUserDetailsAsync(user);
        //     }
        //
        //     return user;
        // }
        //
        // protected virtual async Task LoadUserDetailsAsync(AppUser user)
        // {
        //     if (user == null)
        //     {
        //         throw new ArgumentNullException(nameof(user));
        //     }
        //
        //     // await LoadRelatedRoles(user);
        //     // await LoadRelatedPermissions(user);
        //     // await LoadRelatedRefreshTokens(user);
        //     // await LoadRelatedLogins(user);
        // }
    }
}