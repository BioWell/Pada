using FluentValidation;

namespace Pada.Modules.Identity.Application.Users.Features.Activation
{
    public class ActiveUserCommandValidator: AbstractValidator<ActivateUserCommand>
    {
        public ActiveUserCommandValidator()
        {
            RuleFor(c => c.UserId)
                .NotEmpty().WithMessage("The {PropertyName} field is required.");
        }
    }
}