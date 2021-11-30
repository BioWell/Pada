using MediatR;
using Pada.Modules.Identity.Application.Users.Dtos;

namespace Pada.Modules.Identity.Application.Users.Features.Lock
{
    public class LockUserCommand : IRequest<LockUserResponse>
    {
        public LockUserCommand(string userId)
        {
            UserId = userId;
        }

        public string UserId { get; }
    }
}