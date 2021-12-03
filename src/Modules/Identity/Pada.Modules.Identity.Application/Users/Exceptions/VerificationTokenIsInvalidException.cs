using Pada.Infrastructure.Exceptions;

namespace Pada.Modules.Identity.Application.Users.Exceptions
{
    public class VerificationTokenIsInvalidException : AppException
    {
        public VerificationTokenIsInvalidException(string userId) : base(
            $"verification token is invalid for userId '{userId}'.")
        {
            Code = "verification";
        }
    }
}