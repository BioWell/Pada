using System.Threading.Tasks;
using Pada.Modules.Identity.Application.Users.Contracts;
using Pada.Modules.Identity.Domain.Aggregates.Users;
using Pada.Modules.Identity.Infrastructure.Persistence;

namespace Pada.Modules.Identity.Infrastructure.Services.Users
{
    public class UserRepository : IUserRepository
    {
        private readonly CustomUserManager _userManager;

        public UserRepository(CustomUserManager userManager)
        {
            _userManager = userManager;
        }

        public async Task<User> FindByIdAsync(string id, bool invalidateCache = false)
        {
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
    }
}