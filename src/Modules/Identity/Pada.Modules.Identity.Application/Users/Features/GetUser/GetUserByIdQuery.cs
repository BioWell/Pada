using System;
using MediatR;
using Pada.Modules.Identity.Application.Users.Dtos.GatewayResponses;
using Pada.Modules.Identity.Domain.Aggregates.Users.Types;

namespace Pada.Modules.Identity.Application.Users.Features.GetUser
{
    public class GetUserByIdQuery : IRequest<GetUserResponse>
    {
        public Guid UserId { get; }

        public GetUserByIdQuery(UserId userId)
        {
            UserId = userId;
        }
    }
}