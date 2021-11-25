using System;
using MediatR;
using Pada.Modules.Identity.Application.Users.Dtos.GatewayResponses;

namespace Pada.Modules.Identity.Application.Users.Features.Activation
{
    public class ActivateUserCommand : IRequest<UpdateUserResponse>
    {
        public string UserId { get; }
        public Guid Id { get; set; }

        public ActivateUserCommand(string userId)
        {
            UserId = userId;
        }
    }
}