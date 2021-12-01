using Pada.Infrastructure.Exceptions;

namespace Pada.Modules.Identity.Application.Authentication.Exceptions
{
    public class InvalidRefreshTokenException: AppException
    {
        public InvalidRefreshTokenException() : base("access_token is invalid!")
        {
            Code = "Token";
        }
    }
}