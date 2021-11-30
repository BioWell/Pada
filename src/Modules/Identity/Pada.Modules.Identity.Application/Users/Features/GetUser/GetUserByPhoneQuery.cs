using MediatR;
using Pada.Modules.Identity.Application.Users.Dtos;

namespace Pada.Modules.Identity.Application.Users.Features.GetUser
{
    public class GetUserByPhoneQuery: IRequest<GetUserResponse>
    {
        public string Phone { get; }
        
        public GetUserByPhoneQuery(string phone)
        {
            Phone = phone;
        }
    }
}