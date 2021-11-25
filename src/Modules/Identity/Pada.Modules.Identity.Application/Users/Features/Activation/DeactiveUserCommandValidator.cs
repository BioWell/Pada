using FluentValidation;

namespace Pada.Modules.Identity.Application.Users.Features.Activation
{
    public class DeactiveUserCommandValidator: AbstractValidator<DeActivateUserCommand>
    {
        public DeactiveUserCommandValidator()
        {
            RuleFor(c => c.UserId)
                .NotEmpty().WithMessage("The {PropertyName} field is required.");
        }
    }
}