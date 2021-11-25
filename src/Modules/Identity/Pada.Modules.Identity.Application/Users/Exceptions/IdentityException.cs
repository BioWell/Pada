using Pada.Infrastructure.Exceptions;

namespace Pada.Modules.Identity.Application.Users.Exceptions
{
    public class IdentityException: AppException
    {
        public IdentityException(string reason) : base(
            $"System SignUp '{reason}'.")
        {
        }
    }
}