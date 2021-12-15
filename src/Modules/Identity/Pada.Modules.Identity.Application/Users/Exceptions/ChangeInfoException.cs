using Pada.Infrastructure.Exceptions;

namespace Pada.Modules.Identity.Application.Users.Exceptions
{
    public class ChangeInfoException : AppException
    {
        public ChangeInfoException(string userId) : base($"User '{userId}' is info change fail.")
        {
            Code = "ChangePersonalInfo";
        }
    }
}