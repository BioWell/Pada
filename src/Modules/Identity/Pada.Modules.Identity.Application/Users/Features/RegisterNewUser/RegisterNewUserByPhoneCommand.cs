using MediatR;

namespace Pada.Modules.Identity.Application.Users.Features.RegisterNewUser
{
    public class RegisterNewUserByPhoneCommand: IRequest
    {
        public string PhoneNumber { get; }
        
        public RegisterNewUserByPhoneCommand(string phoneNumber)
        {
            PhoneNumber = phoneNumber;
        }
    }
}