using FluentAssertions;
using Loan_API_project.Enum;
using Loan_API_project.Models.DTO;
using Loan_API_project.Validators;

namespace LoanApi.Tests
{
    public class RegisterDtoValidatorTests
    {
        private readonly RegisterDtoValidator _validator = new();

        [Fact]
        public void Validate_ShouldPass_WhenAllFieldsAreValid()
        {
            // Arrange
            var dto = new RegisterDto
            {
                FirstName = "Giorgi",
                LastName = "Meladze",
                Username = "gmeladze",
                Age = 25,
                Email = "giorgi@test.com",
                MonthlyIncome = 2000,
                Password = "Password123"
            };

            // Act
            var result = _validator.Validate(dto);

            // Assert
            result.IsValid.Should().BeTrue();
        }

        [Fact]
        public void Validate_ShouldFail_WhenFirstNameIsEmpty()
        {
            // Arrange
            var dto = new RegisterDto
            {
                FirstName = "",
                LastName = "Meladze",
                Username = "gmeladze",
                Age = 25,
                Email = "giorgi@test.com",
                MonthlyIncome = 2000,
                Password = "Password123"
            };

            // Act
            var result = _validator.Validate(dto);

            // Assert
            result.IsValid.Should().BeFalse();
            result.Errors.Should().Contain(e => e.PropertyName == "FirstName");
        }

        [Fact]
        public void Validate_ShouldFail_WhenUsernameIsTooShort()
        {
            // Arrange
            var dto = new RegisterDto
            {
                FirstName = "Giorgi",
                LastName = "Meladze",
                Username = "ab", // Only 2 characters
                Age = 25,
                Email = "giorgi@test.com",
                MonthlyIncome = 2000,
                Password = "Password123"
            };

            // Act
            var result = _validator.Validate(dto);

            // Assert
            result.IsValid.Should().BeFalse();
            result.Errors.Should().Contain(e => e.PropertyName == "Username");
        }

        [Fact]
        public void Validate_ShouldFail_WhenUsernameContainsInvalidCharacters()
        {
            // Arrange
            var dto = new RegisterDto
            {
                FirstName = "Giorgi",
                LastName = "Meladze",
                Username = "user@name!", // Invalid characters
                Age = 25,
                Email = "giorgi@test.com",
                MonthlyIncome = 2000,
                Password = "Password123"
            };

            // Act
            var result = _validator.Validate(dto);

            // Assert
            result.IsValid.Should().BeFalse();
            result.Errors.Should().Contain(e => e.PropertyName == "Username");
        }

        [Fact]
        public void Validate_ShouldFail_WhenAgeIsBelow18()
        {
            // Arrange
            var dto = new RegisterDto
            {
                FirstName = "Giorgi",
                LastName = "Meladze",
                Username = "gmeladze",
                Age = 17, // Under 18
                Email = "giorgi@test.com",
                MonthlyIncome = 2000,
                Password = "Password123"
            };

            // Act
            var result = _validator.Validate(dto);

            // Assert
            result.IsValid.Should().BeFalse();
            result.Errors.Should().Contain(e => e.PropertyName == "Age");
        }

        [Fact]
        public void Validate_ShouldFail_WhenAgeIsAbove100()
        {
            // Arrange
            var dto = new RegisterDto
            {
                FirstName = "Giorgi",
                LastName = "Meladze",
                Username = "gmeladze",
                Age = 101, // Over 100
                Email = "giorgi@test.com",
                MonthlyIncome = 2000,
                Password = "Password123"
            };

            // Act
            var result = _validator.Validate(dto);

            // Assert
            result.IsValid.Should().BeFalse();
            result.Errors.Should().Contain(e => e.PropertyName == "Age");
        }

        [Fact]
        public void Validate_ShouldFail_WhenEmailIsInvalid()
        {
            // Arrange
            var dto = new RegisterDto
            {
                FirstName = "Giorgi",
                LastName = "Meladze",
                Username = "gmeladze",
                Age = 25,
                Email = "not-an-email", // Invalid email
                MonthlyIncome = 2000,
                Password = "Password123"
            };

            // Act
            var result = _validator.Validate(dto);

            // Assert
            result.IsValid.Should().BeFalse();
            result.Errors.Should().Contain(e => e.PropertyName == "Email");
        }

        [Fact]
        public void Validate_ShouldFail_WhenMonthlyIncomeIsZero()
        {
            // Arrange
            var dto = new RegisterDto
            {
                FirstName = "Giorgi",
                LastName = "Meladze",
                Username = "gmeladze",
                Age = 25,
                Email = "giorgi@test.com",
                MonthlyIncome = 0, // Zero income
                Password = "Password123"
            };

            // Act
            var result = _validator.Validate(dto);

            // Assert
            result.IsValid.Should().BeFalse();
            result.Errors.Should().Contain(e => e.PropertyName == "MonthlyIncome");
        }

        [Fact]
        public void Validate_ShouldFail_WhenPasswordIsTooShort()
        {
            // Arrange
            var dto = new RegisterDto
            {
                FirstName = "Giorgi",
                LastName = "Meladze",
                Username = "gmeladze",
                Age = 25,
                Email = "giorgi@test.com",
                MonthlyIncome = 2000,
                Password = "Pass1" // Only 5 characters
            };

            // Act
            var result = _validator.Validate(dto);

            // Assert
            result.IsValid.Should().BeFalse();
            result.Errors.Should().Contain(e => e.PropertyName == "Password");
        }

        [Fact]
        public void Validate_ShouldFail_WhenPasswordHasNoUppercase()
        {
            // Arrange
            var dto = new RegisterDto
            {
                FirstName = "Giorgi",
                LastName = "Meladze",
                Username = "gmeladze",
                Age = 25,
                Email = "giorgi@test.com",
                MonthlyIncome = 2000,
                Password = "password123" // No uppercase
            };

            // Act
            var result = _validator.Validate(dto);

            // Assert
            result.IsValid.Should().BeFalse();
            result.Errors.Should().Contain(e => e.PropertyName == "Password");
        }

        [Fact]
        public void Validate_ShouldFail_WhenPasswordHasNoLowercase()
        {
            // Arrange
            var dto = new RegisterDto
            {
                FirstName = "Giorgi",
                LastName = "Meladze",
                Username = "gmeladze",
                Age = 25,
                Email = "giorgi@test.com",
                MonthlyIncome = 2000,
                Password = "PASSWORD123" // No lowercase
            };

            // Act
            var result = _validator.Validate(dto);

            // Assert
            result.IsValid.Should().BeFalse();
            result.Errors.Should().Contain(e => e.PropertyName == "Password");
        }

        [Fact]
        public void Validate_ShouldFail_WhenPasswordHasNoNumber()
        {
            // Arrange
            var dto = new RegisterDto
            {
                FirstName = "Giorgi",
                LastName = "Meladze",
                Username = "gmeladze",
                Age = 25,
                Email = "giorgi@test.com",
                MonthlyIncome = 2000,
                Password = "PasswordABC" // No number
            };

            // Act
            var result = _validator.Validate(dto);

            // Assert
            result.IsValid.Should().BeFalse();
            result.Errors.Should().Contain(e => e.PropertyName == "Password");
        }
    }

    public class LoginDtoValidatorTests
    {
        private readonly LoginDtoValidator _validator = new();

        [Fact]
        public void Validate_ShouldPass_WhenAllFieldsAreValid()
        {
            // Arrange
            var dto = new LoginDto
            {
                Username = "testuser",
                Password = "password"
            };

            // Act
            var result = _validator.Validate(dto);

            // Assert
            result.IsValid.Should().BeTrue();
        }

        [Fact]
        public void Validate_ShouldFail_WhenUsernameIsEmpty()
        {
            // Arrange
            var dto = new LoginDto
            {
                Username = "",
                Password = "password"
            };

            // Act
            var result = _validator.Validate(dto);

            // Assert
            result.IsValid.Should().BeFalse();
            result.Errors.Should().Contain(e => e.PropertyName == "Username");
        }

        [Fact]
        public void Validate_ShouldFail_WhenPasswordIsEmpty()
        {
            // Arrange
            var dto = new LoginDto
            {
                Username = "testuser",
                Password = ""
            };

            // Act
            var result = _validator.Validate(dto);

            // Assert
            result.IsValid.Should().BeFalse();
            result.Errors.Should().Contain(e => e.PropertyName == "Password");
        }
    }

    public class CreateLoanDtoValidatorTests
    {
        private readonly CreateLoanDtoValidator _validator = new();

        [Fact]
        public void Validate_ShouldPass_WhenAllFieldsAreValid()
        {
            // Arrange
            var dto = new CreateLoanDto
            {
                LoanType = LoanType.QuickLoan,
                Amount = 5000,
                Currency = Currency.GEL,
                Period = 12
            };

            // Act
            var result = _validator.Validate(dto);

            // Assert
            result.IsValid.Should().BeTrue();
        }

        [Fact]
        public void Validate_ShouldFail_WhenLoanTypeIsInvalid()
        {
            // Arrange
            var dto = new CreateLoanDto
            {
                LoanType = (LoanType)999, // Invalid enum value
                Amount = 5000,
                Currency = Currency.GEL,
                Period = 12
            };

            // Act
            var result = _validator.Validate(dto);

            // Assert
            result.IsValid.Should().BeFalse();
            result.Errors.Should().Contain(e => e.PropertyName == "LoanType");
        }

        [Fact]
        public void Validate_ShouldFail_WhenAmountIsZero()
        {
            // Arrange
            var dto = new CreateLoanDto
            {
                LoanType = LoanType.QuickLoan,
                Amount = 0,
                Currency = Currency.GEL,
                Period = 12
            };

            // Act
            var result = _validator.Validate(dto);

            // Assert
            result.IsValid.Should().BeFalse();
            result.Errors.Should().Contain(e => e.PropertyName == "Amount");
        }

        [Fact]
        public void Validate_ShouldFail_WhenAmountIsNegative()
        {
            // Arrange
            var dto = new CreateLoanDto
            {
                LoanType = LoanType.QuickLoan,
                Amount = -1000,
                Currency = Currency.GEL,
                Period = 12
            };

            // Act
            var result = _validator.Validate(dto);

            // Assert
            result.IsValid.Should().BeFalse();
            result.Errors.Should().Contain(e => e.PropertyName == "Amount");
        }

        [Fact]
        public void Validate_ShouldFail_WhenAmountExceedsLimit()
        {
            // Arrange
            var dto = new CreateLoanDto
            {
                LoanType = LoanType.QuickLoan,
                Amount = 1500000, // Over 1,000,000
                Currency = Currency.GEL,
                Period = 12
            };

            // Act
            var result = _validator.Validate(dto);

            // Assert
            result.IsValid.Should().BeFalse();
            result.Errors.Should().Contain(e => e.PropertyName == "Amount");
        }

        [Fact]
        public void Validate_ShouldFail_WhenCurrencyIsInvalid()
        {
            // Arrange
            var dto = new CreateLoanDto
            {
                LoanType = LoanType.QuickLoan,
                Amount = 5000,
                Currency = (Currency)999, // Invalid enum
                Period = 12
            };

            // Act
            var result = _validator.Validate(dto);

            // Assert
            result.IsValid.Should().BeFalse();
            result.Errors.Should().Contain(e => e.PropertyName == "Currency");
        }

        [Fact]
        public void Validate_ShouldFail_WhenPeriodIsZero()
        {
            // Arrange
            var dto = new CreateLoanDto
            {
                LoanType = LoanType.QuickLoan,
                Amount = 5000,
                Currency = Currency.GEL,
                Period = 0
            };

            // Act
            var result = _validator.Validate(dto);

            // Assert
            result.IsValid.Should().BeFalse();
            result.Errors.Should().Contain(e => e.PropertyName == "Period");
        }

        [Fact]
        public void Validate_ShouldFail_WhenPeriodExceeds360()
        {
            // Arrange
            var dto = new CreateLoanDto
            {
                LoanType = LoanType.QuickLoan,
                Amount = 5000,
                Currency = Currency.GEL,
                Period = 400 // Over 360 months
            };

            // Act
            var result = _validator.Validate(dto);

            // Assert
            result.IsValid.Should().BeFalse();
            result.Errors.Should().Contain(e => e.PropertyName == "Period");
        }
    }

    public class UpdateLoanDtoValidatorTests
    {
        private readonly UpdateLoanDtoValidator _validator = new();

        [Fact]
        public void Validate_ShouldPass_WhenAllFieldsAreValid()
        {
            // Arrange
            var dto = new UpdateLoanDto
            {
                LoanType = LoanType.AutoLoan,
                Amount = 6000,
                Currency = Currency.USD,
                Period = 18
            };

            // Act
            var result = _validator.Validate(dto);

            // Assert
            result.IsValid.Should().BeTrue();
        }

        [Fact]
        public void Validate_ShouldPass_WhenAllFieldsAreNull()
        {
            // Arrange
            var dto = new UpdateLoanDto
            {
                LoanType = null,
                Amount = null,
                Currency = null,
                Period = null
            };

            // Act
            var result = _validator.Validate(dto);

            // Assert
            result.IsValid.Should().BeTrue();
        }

        [Fact]
        public void Validate_ShouldFail_WhenAmountIsNegative()
        {
            // Arrange
            var dto = new UpdateLoanDto
            {
                Amount = -500
            };

            // Act
            var result = _validator.Validate(dto);

            // Assert
            result.IsValid.Should().BeFalse();
            result.Errors.Should().Contain(e => e.PropertyName == "Amount");
        }

        [Fact]
        public void Validate_ShouldFail_WhenPeriodIsInvalid()
        {
            // Arrange
            var dto = new UpdateLoanDto
            {
                Period = 400 // Over 360
            };

            // Act
            var result = _validator.Validate(dto);

            // Assert
            result.IsValid.Should().BeFalse();
            result.Errors.Should().Contain(e => e.PropertyName == "Period");
        }

        [Fact]
        public void Validate_ShouldPass_WhenOnlyOneFieldIsUpdated()
        {
            // Arrange
            var dto = new UpdateLoanDto
            {
                Amount = 7000
            };

            // Act
            var result = _validator.Validate(dto);

            // Assert
            result.IsValid.Should().BeTrue();
        }
    }
}