using MediatR;

namespace Pada.Modules.Identity.Application.Users.Features.RegisterNewUser
{
    public class SendVerificationEmailCommand : IRequest
    {
        public string UserId { get; }

        public SendVerificationEmailCommand(string userId)
        {
            UserId = userId;
        }
    }
}