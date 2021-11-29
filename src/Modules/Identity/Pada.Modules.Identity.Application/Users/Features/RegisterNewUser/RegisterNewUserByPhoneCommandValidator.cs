using FluentValidation;
using Pada.Infrastructure.Utils;

namespace Pada.Modules.Identity.Application.Users.Features.RegisterNewUser
{
    public class RegisterNewUserByPhoneCommandValidator: AbstractValidator<RegisterNewUserByPhoneCommand>
    {
        public RegisterNewUserByPhoneCommandValidator()
        {
            RuleFor(c => c.PhoneNumber)
                .NotEmpty().WithMessage("The {PropertyName} field is required.")
                .Must(ValidatorHelper.ValidPhone).WithMessage("Invalid Email Address.");            
        }
    }
}