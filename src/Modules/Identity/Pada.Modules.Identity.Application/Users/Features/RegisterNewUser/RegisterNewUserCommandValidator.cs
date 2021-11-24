using FluentValidation;

namespace Pada.Modules.Identity.Application.Users.Features.RegisterNewUser
{
    public class RegisterNewUserCommandValidator: AbstractValidator<RegisterNewUserCommand>
    {
        public RegisterNewUserCommandValidator()
        {
            RuleFor(x => x.Name).NotEmpty().WithMessage("UserNameOrEmail cannot be empty");
            RuleFor(x => x.Password).NotEmpty().WithMessage("password cannot be empty");
        }
    }
}