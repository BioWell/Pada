using Pada.Infrastructure.Exceptions;

namespace Pada.Modules.Identity.Application.Users.Exceptions
{
    public class UserPhoneAlreadyInUseException: AppException
    {
        public string Phone { get; }

        public UserPhoneAlreadyInUseException(string phone) : base($"Phone '{phone}' is already in used.")
        {
            Phone = phone;
            Code = nameof(Phone);
        }
    }
}