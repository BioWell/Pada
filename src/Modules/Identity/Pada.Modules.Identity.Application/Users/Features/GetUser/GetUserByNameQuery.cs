using MediatR;
using Pada.Modules.Identity.Application.Users.Dtos;

namespace Pada.Modules.Identity.Application.Users.Features.GetUser
{
    public class GetUserByUserNameQuery : IRequest<GetUserResponse>
    {
        public string UserName { get; }

        public GetUserByUserNameQuery(string userName)
        {
            UserName = userName;
        }
    }
}