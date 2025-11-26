using FluentValidation;
using Loan_API_project.Models.DTO;

namespace Loan_API_project.Validators
{
    public class UpdateLoanDtoValidator : AbstractValidator<UpdateLoanDto>
    {
        public UpdateLoanDtoValidator()
        {
            RuleFor(x => x.LoanType)
                .IsInEnum().WithMessage("არასწორი სესხის ტიპი")
                .When(x => x.LoanType.HasValue);

            RuleFor(x => x.Amount)
                .GreaterThan(0).WithMessage("თანხა უნდა იყოს დადებითი რიცხვი")
                .LessThanOrEqualTo(1000000).WithMessage("თანხა არ უნდა აღემატებოდეს 1,000,000-ს")
                .When(x => x.Amount.HasValue);

            RuleFor(x => x.Currency)
                .IsInEnum().WithMessage("არასწორი ვალუტა")
                .When(x => x.Currency.HasValue);

            RuleFor(x => x.Period)
                .InclusiveBetween(1, 360).WithMessage("პერიოდი უნდა იყოს 1-დან 360 თვემდე")
                .When(x => x.Period.HasValue);
        }
    }
}
