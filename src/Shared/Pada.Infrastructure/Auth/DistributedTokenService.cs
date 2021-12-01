using System;
using System.Linq;
using System.Threading.Tasks;
using EasyCaching.Core;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Primitives;
using Pada.Abstractions.Auth;

namespace Pada.Infrastructure.Auth
{
    public class DistributedTokenService: IAccessTokenService
    {
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly IEasyCachingProvider _cacheProvider;
        private readonly TimeSpan _expires;

        public DistributedTokenService(IEasyCachingProviderFactory cachingFactory,
            IHttpContextAccessor httpContextAccessor,
            JwtOptions jwtOptions)
        {
            _cacheProvider = cachingFactory.GetCachingProvider("redis");
            _httpContextAccessor = httpContextAccessor;
            _expires =  TimeSpan.FromMinutes(jwtOptions.ExpiryMinutes);
        }
        public Task<bool> IsCurrentActiveToken()
        {
            return IsActiveAsync(GetCurrentAsync());
        }

        public Task DeactivateCurrentAsync()
        {
            return DeactivateAsync(GetCurrentAsync());
        }

        public async Task<bool> IsActiveAsync(string token)
        {
            return string.IsNullOrWhiteSpace((await _cacheProvider.GetAsync<string>(GetKey(token))).Value);
        }

        public Task DeactivateAsync(string token)
        {
            return _cacheProvider.SetAsync(GetKey(token), "revoked", _expires);
        }

        private string GetCurrentAsync()
        {
            var authorizationHeader = _httpContextAccessor.HttpContext?.Request.Headers.Get<string>("authorization");

            return authorizationHeader is null|| authorizationHeader == StringValues.Empty
                ? string.Empty
                : authorizationHeader.Split(' ').Last();
        }

        private static string GetKey(string token)
        {
            return $"blacklisted-tokens:{token}";
        }
    }
}