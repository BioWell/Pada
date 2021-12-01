using Pada.Infrastructure.Exceptions;

namespace Pada.Modules.Identity.Application.Authentication.Exceptions
{
    public class PasswordIsInvalidException : AppException
    {
        public PasswordIsInvalidException() : base("password is invalid!")
        {
            Code = "Password";
        }
    }
}