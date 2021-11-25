using System;
using MediatR;
using Pada.Modules.Identity.Application.Users.Dtos.UseCaseResponses;
using Pada.Modules.Identity.Domain.Aggregates.Users.Types;

namespace Pada.Modules.Identity.Application.Users.Features.GetUser
{
    public class GetUserByIdQuery : IRequest<UserDto>
    {
        public GetUserByIdQuery(UserId userId)
        {
            UserId = userId;
        }

        public Guid UserId { get; }
    }
}