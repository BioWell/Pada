using MediatR;

namespace Pada.Modules.Identity.Application.Users.Features.RegisterNewUser
{
    public class SendVerificationEmailCommand : IRequest
    {
        public string UserId { get; }
        public string RequestScheme { get; }
        public string RequestHost { get; }

        public SendVerificationEmailCommand(string userId, string requestScheme, string requestHost)
        {
            UserId = userId;
            RequestScheme = requestScheme;
            RequestHost = requestHost;
        }
    }
}