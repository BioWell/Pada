using FluentValidation;
using Pada.Infrastructure.Utils;

namespace Pada.Modules.Identity.Application.Users.Features.GetUser
{
    public class GetUserByEmailValidator: AbstractValidator<GetUserByEmailQuery>
    {
        public GetUserByEmailValidator()
        {
            RuleFor(c => c.Email)
                .NotEmpty().WithMessage("The {PropertyName} field is required.")
                .EmailAddress().WithMessage("Invalid Email Address.");
        }
    }
    
    public class GetUserByPhoneValidator: AbstractValidator<GetUserByPhoneQuery>
    {
        public GetUserByPhoneValidator()
        {
            RuleFor(c => c.Phone)
                .NotEmpty().WithMessage("The {PropertyName} field is required.")
                .Must(ValidatorHelper.ValidPhone).WithMessage("Invalid Email Address.");
        }
    }
    
}