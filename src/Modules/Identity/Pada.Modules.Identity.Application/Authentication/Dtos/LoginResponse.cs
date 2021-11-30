using Microsoft.IdentityModel.JsonWebTokens;

namespace Pada.Modules.Identity.Application.Authentication.Dtos
{
    public class LoginResponse
    {
        public LoginResponse(JsonWebToken jwtToken, string refreshToken)
        {
            JsonWebToken = jwtToken;
            RefreshToken = refreshToken;
        }
        public JsonWebToken JsonWebToken { get; }
        public string RefreshToken { get; }
    }
}