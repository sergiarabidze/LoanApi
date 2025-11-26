using FluentValidation;
using Loan_API_project.Models.DTO;

namespace Loan_API_project.Validators
{
    public class LoginDtoValidator : AbstractValidator<LoginDto>
    {
        public LoginDtoValidator()
        {
            RuleFor(x => x.Username)
                .NotEmpty().WithMessage("მომხმარებლის სახელი აუცილებელია");

            RuleFor(x => x.Password)
                .NotEmpty().WithMessage("პაროლი აუცილებელია");
        }
    }
}
