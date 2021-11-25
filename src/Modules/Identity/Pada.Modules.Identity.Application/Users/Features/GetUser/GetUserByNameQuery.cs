using MediatR;
using Pada.Modules.Identity.Application.Users.Dtos.UseCaseResponses;

namespace Pada.Modules.Identity.Application.Users.Features.GetUser
{
    public class GetUserByUserNameQuery : IRequest<UserDto>
    {
        public GetUserByUserNameQuery(string userName)
        {
            UserName = userName;
        }

        public string UserName { get; }
    }
}