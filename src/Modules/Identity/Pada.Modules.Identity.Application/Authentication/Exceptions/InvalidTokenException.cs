using Pada.Infrastructure.Exceptions;

namespace Pada.Modules.Identity.Application.Authentication.Exceptions
{
    public class InvalidTokenException: AppException
    {
        public InvalidTokenException() : base("access_token is invalid!")
        {
            Code = "Token";
        }
    }
}