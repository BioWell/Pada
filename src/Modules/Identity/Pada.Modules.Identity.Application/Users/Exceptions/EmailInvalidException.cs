using Pada.Infrastructure.Exceptions;

namespace Pada.Modules.Identity.Application.Users.Exceptions
{
    public class EmailInvalidException: AppException
    {
        public string Email { get; }

        public EmailInvalidException(string email) : base($"Email '{email}' is invalid.")
        {
            Code = nameof(Email);
            Email = email;
        }
    }
}