using MediatR;
using Pada.Modules.Identity.Application.Users.Dtos.GatewayResponses;

namespace Pada.Modules.Identity.Application.Users.Features.GetUser
{
    public class GetUserByEmailQuery : IRequest<GetUserResponse>
    {
        public string Email { get; }

        public GetUserByEmailQuery(string email)
        {
            Email = email;
        }
    }
}