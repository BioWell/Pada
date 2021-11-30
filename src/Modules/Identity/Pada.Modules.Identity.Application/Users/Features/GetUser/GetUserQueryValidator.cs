using FluentValidation;
using Pada.Infrastructure.Utils;

namespace Pada.Modules.Identity.Application.Users.Features.GetUser
{
    public class GetUserByEmailValidator : AbstractValidator<GetUserByEmailQuery>
    {
        public GetUserByEmailValidator()
        {
            RuleFor(c => c.Email)
                .NotEmpty().WithMessage("The {PropertyName} field is required.")
                .EmailAddress().WithMessage("Invalid Email Address.");
        }
    }

    public class GetUserByPhoneValidator : AbstractValidator<GetUserByPhoneQuery>
    {
        public GetUserByPhoneValidator()
        {
            RuleFor(c => c.Phone)
                .NotEmpty().WithMessage("The {PropertyName} field is required.")
                .Must(ValidatorHelper.ValidPhone).WithMessage("Invalid Phone.");
        }
    }

    public class GetUserByNameValidator : AbstractValidator<GetUserByUserNameQuery>
    {
        public GetUserByNameValidator()
        {
            RuleFor(c => c.UserName)
                .NotEmpty().WithMessage("The {PropertyName} field is required.");
        }
    }

    public class GetUserByIdValidator : AbstractValidator<GetUserByIdQuery>
    {
        public GetUserByIdValidator()
        {
            RuleFor(c => c.UserId)
                .NotEmpty().WithMessage("The {PropertyName} field is required.")
                .Must(ValidatorHelper.ValidId).WithMessage("Invalid Guid ID.");
        }
    }
}