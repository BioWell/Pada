using Pada.Infrastructure.Exceptions;

namespace Pada.Modules.Identity.Application.Users.Exceptions
{
    public class SendVerificationEmailFailedException : AppException
    {
        public SendVerificationEmailFailedException(string userId) : base(
            $"sending verification email for UserId '{userId}' failed.")
        {
            Code = "Verification";
        }
    }
}