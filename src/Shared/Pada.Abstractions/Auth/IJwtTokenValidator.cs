using System.Security.Claims;

namespace Pada.Abstractions.Auth
{
    public interface IJwtTokenValidator
    {
        ClaimsPrincipal GetPrincipalFromToken(string token);
    }
}