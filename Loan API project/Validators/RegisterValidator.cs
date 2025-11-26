using FluentValidation;
using Loan_API_project.Models.DTO;

namespace Loan_API_project.Validators
{
    public class RegisterDtoValidator : AbstractValidator<RegisterDto>
    {
        public RegisterDtoValidator()
        {
            RuleFor(x => x.FirstName)
                .NotEmpty().WithMessage("სახელი აუცილებელია")
                .MaximumLength(100).WithMessage("სახელი არ უნდა აღემატებოდეს 100 სიმბოლოს");

            RuleFor(x => x.LastName)
                .NotEmpty().WithMessage("გვარი აუცილებელია")
                .MaximumLength(100).WithMessage("გვარი არ უნდა აღემატებოდეს 100 სიმბოლოს");

            RuleFor(x => x.Username)
                .NotEmpty().WithMessage("მომხმარებლის სახელი აუცილებელია")
                .MinimumLength(3).WithMessage("მომხმარებლის სახელი უნდა იყოს მინიმუმ 3 სიმბოლო")
                .MaximumLength(50).WithMessage("მომხმარებლის სახელი არ უნდა აღემატებოდეს 50 სიმბოლოს")
                .Matches("^[a-zA-Z0-9_]+$").WithMessage("მომხმარებლის სახელი უნდა შეიცავდეს მხოლოდ ასოებს, რიცხვებს და ხაზს");

            RuleFor(x => x.Age)
                .InclusiveBetween(18, 100).WithMessage("ასაკი უნდა იყოს 18-დან 100-მდე");

            RuleFor(x => x.Email)
                .NotEmpty().WithMessage("ელ. ფოსტა აუცილებელია")
                .EmailAddress().WithMessage("არასწორი ელ. ფოსტის ფორმატი")
                .MaximumLength(100).WithMessage("ელ. ფოსტა არ უნდა აღემატებოდეს 100 სიმბოლოს");

            RuleFor(x => x.MonthlyIncome)
                .GreaterThan(0).WithMessage("ყოველთვიური შემოსავალი უნდა იყოს დადებითი რიცხვი");

            RuleFor(x => x.Password)
                .NotEmpty().WithMessage("პაროლი აუცილებელია")
                .MinimumLength(6).WithMessage("პაროლი უნდა იყოს მინიმუმ 6 სიმბოლო")
                .Matches("[A-Z]").WithMessage("პაროლი უნდა შეიცავდეს მინიმუმ ერთ დიდ ასოს")
                .Matches("[a-z]").WithMessage("პაროლი უნდა შეიცავდეს მინიმუმ ერთ პატარა ასოს")
                .Matches("[0-9]").WithMessage("პაროლი უნდა შეიცავდეს მინიმუმ ერთ ციფრს");
        }
    }

}
