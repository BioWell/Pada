using Pada.Infrastructure.Exceptions;

namespace Pada.Modules.Identity.Application.Users.Exceptions
{
    public class UserInactiveException : AppException
    {
        public string UserName { get; }

        public UserInactiveException(string userName) : base($"username {userName} is inactive.")
        {
            Code = nameof(UserName);
            UserName = userName;
        }
    }
}