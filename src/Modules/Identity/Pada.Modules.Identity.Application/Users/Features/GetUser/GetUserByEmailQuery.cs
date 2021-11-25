using MediatR;
using Pada.Modules.Identity.Application.Users.Dtos.UseCaseResponses;

namespace Pada.Modules.Identity.Application.Users.Features.GetUser
{
    public class GetUserByEmailQuery : IRequest<UserDto>
    {
        public GetUserByEmailQuery(string email)
        {
            Email = email;
        }

        public string Email { get; }
    }
}