using Pada.Infrastructure.Exceptions;

namespace Pada.Modules.Identity.Application.Users.Exceptions
{
    public class UserLockedException : AppException
    {
        public UserLockedException(string userId) : base($"userId '{userId}' has been locked.")
        {
            Code = "UserLock";
        }
    }
}