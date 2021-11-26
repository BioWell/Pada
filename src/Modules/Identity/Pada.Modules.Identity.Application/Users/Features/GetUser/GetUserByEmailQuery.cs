using MediatR;
using Pada.Modules.Identity.Application.Users.Dtos.GatewayResponses;

namespace Pada.Modules.Identity.Application.Users.Features.GetUser
{
    public class GetUserByEmailQuery : IRequest<GetUserResponse>
    {
        public GetUserByEmailQuery(string email)
        {
            Email = email;
        }

        public string Email { get; }
    }
}