using Microsoft.IdentityModel.JsonWebTokens;
using Pada.Abstractions.Auth;

namespace Pada.Modules.Identity.Application.Authentication.Dtos
{
    public class RefreshTokenResponse
    {
        public RefreshTokenResponse(AppJsonWebToken jwtToken, string refreshToken)
        {
            JsonWebToken = jwtToken;
            RefreshToken = refreshToken;
        }
        public AppJsonWebToken JsonWebToken { get; }
        public string RefreshToken { get; }
    }
}