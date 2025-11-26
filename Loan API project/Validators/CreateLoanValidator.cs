using FluentValidation;
using Loan_API_project.Models.DTO;

namespace Loan_API_project.Validators
{
    public class CreateLoanDtoValidator : AbstractValidator<CreateLoanDto>
    {
        public CreateLoanDtoValidator()
        {
            RuleFor(x => x.LoanType)
                .IsInEnum().WithMessage("არასწორი სესხის ტიპი");

            RuleFor(x => x.Amount)
                .GreaterThan(0).WithMessage("თანხა უნდა იყოს დადებითი რიცხვი")
                .LessThanOrEqualTo(1000000).WithMessage("თანხა არ უნდა აღემატებოდეს 1,000,000-ს");

            RuleFor(x => x.Currency)
                .IsInEnum().WithMessage("არასწორი ვალუტა");

            RuleFor(x => x.Period)
                .InclusiveBetween(1, 360).WithMessage("პერიოდი უნდა იყოს 1-დან 360 თვემდე");
        }
    }
}
