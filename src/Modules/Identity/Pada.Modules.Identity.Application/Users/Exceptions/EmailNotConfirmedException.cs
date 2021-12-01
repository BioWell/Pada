using Pada.Infrastructure.Exceptions;

namespace Pada.Modules.Identity.Application.Users.Exceptions
{
    public class EmailNotConfirmedException : AppException
    {
        public string Email { get; }

        public EmailNotConfirmedException(string email) : base($"Email not confirmed for email address `{email}`")
        {
            Code = nameof(Email);
            Email = email;
        }
    }
}