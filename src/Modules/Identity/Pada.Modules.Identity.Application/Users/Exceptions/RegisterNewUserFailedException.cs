using Pada.Infrastructure.Exceptions;

namespace Pada.Modules.Identity.Application.Users.Exceptions
{
    public class RegisterNewUserFailedException : AppException
    {
        public RegisterNewUserFailedException(string Name) : base(
            $"Register user with name '{Name}' failed.")
        {
        }
    }
}