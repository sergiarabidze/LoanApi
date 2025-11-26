using Loan_API_project.Enum;

namespace Loan_API_project.Models.DTO
{
    public class RegisterDto
    {
        public string FirstName { get; set; } = string.Empty;
        public string LastName { get; set; } = string.Empty;
        public string Username { get; set; } = string.Empty;
        public int Age { get; set; }
        public string Email { get; set; } = string.Empty;
        public decimal MonthlyIncome { get; set; }
        public string Password { get; set; } = string.Empty;
    }

    public class LoginDto
    {
        public string Username { get; set; } = string.Empty;
        public string Password { get; set; } = string.Empty;
    }

    public class LoginResponseDto
    {
        public string Token { get; set; } = string.Empty;
        public UserDto User { get; set; } = null!;
    }

    //User DTOs
    public class UserDto
    {
        public int Id { get; set; }
        public string FirstName { get; set; } = string.Empty;
        public string LastName { get; set; } = string.Empty;
        public string Username { get; set; } = string.Empty;
        public int Age { get; set; }
        public string Email { get; set; } = string.Empty;
        public decimal MonthlyIncome { get; set; }
        public bool IsBlocked { get; set; }
        public string Role { get; set; } = string.Empty;
    }

    //Loan DTOs
    public class CreateLoanDto
    {
        public LoanType LoanType { get; set; }
        public decimal Amount { get; set; }
        public Currency Currency { get; set; }
        public int Period { get; set; }
    }

    public class UpdateLoanDto
    {
        public LoanType? LoanType { get; set; }
        public decimal? Amount { get; set; }
        public Currency? Currency { get; set; }
        public int? Period { get; set; }
    }

    public class LoanDto
    {
        public int Id { get; set; }
        public LoanType LoanType { get; set; }
        public decimal Amount { get; set; }
        public Currency Currency { get; set; }
        public int Period { get; set; }
        public LoanStatus Status { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }
        public int UserId { get; set; }
    }

    public class UpdateLoanStatusDto
    {
        public LoanStatus Status { get; set; }
    }

    public class BlockUserDto
    {
        public bool IsBlocked { get; set; }
    }

    // Response DTOs 
    public class ApiResponse<T>
    {
        public bool Success { get; set; }
        public string Message { get; set; } = string.Empty;
        public T? Data { get; set; }
    }

    public class ErrorResponse
    {
        public int StatusCode { get; set; }
        public string Message { get; set; } = string.Empty;
        public string? Details { get; set; }
    }
}
