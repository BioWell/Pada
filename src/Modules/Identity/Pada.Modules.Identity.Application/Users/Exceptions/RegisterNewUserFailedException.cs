using Pada.Infrastructure.Exceptions;

namespace Pada.Modules.Identity.Application.Users.Exceptions
{
    public class RegisterNewUserFailedException : AppException
    {
        public RegisterNewUserFailedException(string userName) : base(
            $"Register user with username '{userName}' failed.")
        {
        }
    }
}