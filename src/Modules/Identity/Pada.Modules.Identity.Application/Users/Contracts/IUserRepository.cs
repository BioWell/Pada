using System.Threading.Tasks;
using Pada.Modules.Identity.Application.Users.Dtos.GatewayResponses;
using Pada.Modules.Identity.Domain.Aggregates.Users;

namespace Pada.Modules.Identity.Application.Users.Contracts
{
    public interface IUserRepository
    {
        Task<User> FindByIdAsync(string id, bool invalidateCache = false);
        Task<User> FindByEmailAsync(string email, bool invalidateCache = false);
        Task<User> FindByNameAsync(string userName, bool invalidateCache = false);
        Task<User> FindByPhoneAsync(string userName, bool invalidateCache = false);
        bool IsPhoneUsedAsync(string id);
        Task<CreateUserResponse> AddAsync(User user);
        Task<UpdateUserResponse> UpdateAsync(User user);
        Task<LockUserResponse> LockUserAsync(string userId);
        
        // public Task<UnlockUserResponse> UnlockUserAsync(string userId);
    }
}