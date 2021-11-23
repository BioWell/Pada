using Pada.Infrastructure.Exceptions;

namespace Pada.Modules.Identity.Application.Users.Exceptions
{
    public class EmailAlreadyInUsedException : AppException
    {
        public string Email { get; }

        public EmailAlreadyInUsedException(string email) : base($"Email '{email}' is already in use.")
        {
            Email = email;
        }
    }
}