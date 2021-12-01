using System;
using System.Threading.Tasks;
using Pada.Abstractions.Auth;
using Pada.Abstractions.Services.Storage;
using Pada.Abstractions.Services.Token;

namespace Pada.Infrastructure.Services.Token
{
    public class TokenStorageService : ITokenStorageService
    {
        private readonly IRequestStorage _requestStorage;

        public TokenStorageService(IRequestStorage requestStorage)
        {
            _requestStorage = requestStorage;
        }

        public async Task SetAsync(Guid commandId, AppJsonWebToken token)
        {
            await _requestStorage.Set(GetKey(commandId), token);
        }

        public async Task<AppJsonWebToken> GetAsync(Guid commandId)
        {
            return  await _requestStorage.Get<AppJsonWebToken>(GetKey(commandId));
        }
        
        private string GetKey(Guid commandId) => $"users:tokens:{commandId}";
    }
}