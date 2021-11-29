using Pada.Infrastructure.Exceptions;

namespace Pada.Modules.Identity.Application.Users.Exceptions
{
    public class PhoneIsInvalidException : AppException
    {
        public string Phone { get; }

        public PhoneIsInvalidException(string phone) : base($"Phone '{phone}' is invalid.")
        {
            Code = nameof(Phone);
            Phone = phone;
        }
    }
}