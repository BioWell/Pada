using System.Collections.Generic;
using System.Security.Claims;
using System.Threading.Tasks;
using Pada.Modules.Identity.Application.Users.Dtos;
using Pada.Modules.Identity.Domain.Aggregates.Users;

namespace Pada.Modules.Identity.Application.Users.Contracts
{
    public interface IUserRepository
    {
        Task<User> FindByIdAsync(string id, bool invalidateCache = false);
        Task<User> FindByEmailAsync(string email, bool invalidateCache = false);
        Task<User> FindByNameAsync(string userName, bool invalidateCache = false);
        Task<User> FindByPhoneAsync(string userName, bool invalidateCache = false);
        Task<User> FindByNameOrEmailAsync(string userNameOrEmail, bool invalidateCache = false);
        Task<User> FindByRefreshToken(string refreshToken);
        Task<CreateUserResponse> AddAsync(User user);
        Task<UpdateUserResponse> UpdateAsync(User user);
        Task<LockUserResponse> LockUserAsync(string userId);
        Task<CheckPasswordResponse> CheckPassword(string userName, string password);
        Task<(IList<Claim> UserClaims, IList<string> Roles, IList<string> PermissionClaims)> GetClaimsAsync(string userName);
        Task<GenerateEmailConfirmationTokenResponse> GenerateEmailConfirmationTokenAsync(string userId);
        Task<ConfirmEmailResponse> ConfirmEmailAsync(string userId, string verificationCode);
        bool IsPhoneUsedAsync(string id);
        public Task<(User, bool)> IsUserLockedAsync(string userNameOrEmail);
        
        // public Task<UnlockUserResponse> UnlockUserAsync(string userId);
    }
}