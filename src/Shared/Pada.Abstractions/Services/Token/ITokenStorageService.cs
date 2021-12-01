using System;
using System.Threading.Tasks;
using Pada.Abstractions.Auth;

namespace Pada.Abstractions.Services.Token
{
    public interface ITokenStorageService
    {
        Task SetAsync(Guid commandId, AppJsonWebToken token);
        Task<AppJsonWebToken> GetAsync(Guid commandId);
    }
}