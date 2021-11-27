using Pada.Infrastructure.Exceptions;

namespace Pada.Modules.Identity.Application.Users.Exceptions
{
    public class LockUserFailedException : AppException
    {
        public LockUserFailedException(string userId) : base($"Lock user failed for userId '{userId}'")
        {
            Code = "LockUser";
        }
    }
}