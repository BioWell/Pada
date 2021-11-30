using FluentValidation;
using Microsoft.Extensions.Localization;
using Pada.Infrastructure.Utils;

namespace Pada.Modules.Identity.Application.Users.Features.RegisterNewUser
{
    public class RegisterNewUserCommandValidator : AbstractValidator<RegisterNewUserCommand>
    {
        public RegisterNewUserCommandValidator()
        {
            RuleFor(c => c.Name)
                .NotEmpty().WithMessage("The {PropertyName} field is required.")
                .Length(2, 150).WithMessage("The {PropertyName} property must have between 2 and 150 characters.");
            RuleFor(c => c.Password)
                .NotEmpty().WithMessage("The {PropertyName} field is required.")
                .Length(6, 30).WithMessage("The {PropertyName} must be great than 6 characters.");
            RuleFor(c => c.Email)
                .NotEmpty().WithMessage("The {PropertyName} field is required.")
                .EmailAddress().WithMessage("Invalid Email Address.");
        }
    }

    public class RegisterNewUserByPhoneCommandValidator : AbstractValidator<RegisterNewUserByPhoneCommand>
    {
        public RegisterNewUserByPhoneCommandValidator()
        {
            RuleFor(c => c.PhoneNumber)
                .NotEmpty().WithMessage("The {PropertyName} field is required.")
                .Must(ValidatorHelper.ValidPhone).WithMessage("Invalid Phone.");
        }
    }
}