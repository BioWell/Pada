﻿using System.Threading.Tasks;
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
        Task<CreateUserResponse> AddAsync(User user);
        Task<UpdateUserResponse> UpdateAsync(User user);
        Task<LockUserResponse> LockUserAsync(string userId);
        
        bool IsPhoneUsedAsync(string id);
        
        // public Task<UnlockUserResponse> UnlockUserAsync(string userId);
    }
}