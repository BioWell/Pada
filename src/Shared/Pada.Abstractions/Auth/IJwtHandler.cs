using System.Collections.Generic;
using System.Security.Claims;
using Microsoft.IdentityModel.Tokens;

namespace Pada.Abstractions.Auth
{
    public interface IJwtHandler
    {
        public AppJsonWebToken GenerateJwtToken(
            string userName,
            string email,
            string userId,
            bool? isVerified = null,
            string firstName = null,
            string lastName = null,
            string phoneNumber = null,
            IList<Claim> usersClaims = null,
            IList<string> rolesClaims = null,
            IList<string> permissionsClaims = null);

        ClaimsPrincipal ValidateToken(string token, TokenValidationParameters tokenValidationParameters);
        JsonWebTokenPayload GetTokenPayload(string accessToken);
        AppRefreshToken GenerateRefreshToken(string userId, string ipAddress = null);
    }
}