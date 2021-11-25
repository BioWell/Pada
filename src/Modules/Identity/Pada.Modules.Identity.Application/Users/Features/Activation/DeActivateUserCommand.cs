using System;
using MediatR;
using Pada.Modules.Identity.Application.Users.Dtos.GatewayResponses;

namespace Pada.Modules.Identity.Application.Users.Features.Activation
{
    public class DeActivateUserCommand : IRequest<UpdateUserResponse>
    {
        public string UserId { get; }
        public Guid Id { get; set; }

        public DeActivateUserCommand(string userId)
        {
            UserId = userId;
        }
    }
}