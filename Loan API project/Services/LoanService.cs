using Loan_API_project.Enum;
using Loan_API_project.Exceptions;
using Loan_API_project.Models.DTO;
using Loan_API_project.Models.Entities;
using Loan_API_project.Repositories;

namespace Loan_API_project.Services
{
    public interface ILoanService
    {
        Task<LoanDto> CreateLoanAsync(int userId, CreateLoanDto createLoanDto);
        Task<IEnumerable<LoanDto>> GetUserLoansAsync(int userId);
        Task<LoanDto> GetLoanByIdAsync(int userId, int loanId);
        Task<LoanDto> UpdateLoanAsync(int userId, int loanId, UpdateLoanDto updateLoanDto);
        Task DeleteLoanAsync(int userId, int loanId);

        // Accountant methods
        Task<IEnumerable<LoanDto>> GetAllLoansAsync();
        Task<LoanDto> GetAnyLoanByIdAsync(int loanId);
        Task<LoanDto> UpdateAnyLoanAsync(int loanId, UpdateLoanDto updateLoanDto);
        Task<LoanDto> UpdateLoanStatusAsync(int loanId, LoanStatus status);
        Task DeleteAnyLoanAsync(int loanId);
    }
    public class LoanService : ILoanService
    {
        private readonly ILoanRepository _loanRepository;
        private readonly IUserRepository _userRepository;
        private readonly ILogger<LoanService> _logger;

        public LoanService(ILoanRepository loanRepository, IUserRepository userRepository, ILogger<LoanService> logger)
        {
            _loanRepository = loanRepository;
            _userRepository = userRepository;
            _logger = logger;
        }

        public async Task<LoanDto> CreateLoanAsync(int userId, CreateLoanDto createLoanDto)
        {
            var user = await _userRepository.GetByIdAsync(userId);
            if (user == null)
            {
                throw new NotFoundException("მომხმარებელი ვერ მოიძებნა");
            }

            if (user.IsBlocked)
            {
                _logger.LogWarning("Blocked user {UserId} attempted to create loan", userId);
                throw new ForbiddenException("თქვენ დაბლოკილი ხართ და არ შეგიძლიათ სესხის მოთხოვნა");
            }

            var loan = new Loan
            {
                UserId = userId,
                LoanType = createLoanDto.LoanType,
                Amount = createLoanDto.Amount,
                Currency = createLoanDto.Currency,
                Period = createLoanDto.Period,
                Status = LoanStatus.InProcess
            };

            var createdLoan = await _loanRepository.CreateAsync(loan);
            _logger.LogInformation("Loan {LoanId} created for user {UserId}", createdLoan.Id, userId);

            return MapToLoanDto(createdLoan);
        }

        public async Task<IEnumerable<LoanDto>> GetUserLoansAsync(int userId)
        {
            var loans = await _loanRepository.GetByUserIdAsync(userId);
            return loans.Select(MapToLoanDto);
        }

        public async Task<LoanDto> GetLoanByIdAsync(int userId, int loanId)
        {
            var loan = await _loanRepository.GetByIdAsync(loanId);
            if (loan == null)
            {
                throw new NotFoundException("სესხი ვერ მოიძებნა");
            }

            if (loan.UserId != userId)
            {
                _logger.LogWarning("User {UserId} attempted to access loan {LoanId} of another user", userId, loanId);
                throw new ForbiddenException("თქვენ არ გაქვთ წვდომა ამ სესხზე");
            }

            return MapToLoanDto(loan);
        }

        public async Task<LoanDto> UpdateLoanAsync(int userId, int loanId, UpdateLoanDto updateLoanDto)
        {
            var loan = await _loanRepository.GetByIdAsync(loanId);
            if (loan == null)
            {
                throw new NotFoundException("სესხი ვერ მოიძებნა");
            }

            if (loan.UserId != userId)
            {
                _logger.LogWarning("User {UserId} attempted to update loan {LoanId} of another user", userId, loanId);
                throw new ForbiddenException("თქვენ არ გაქვთ წვდომა ამ სესხზე");
            }

            if (loan.Status != LoanStatus.InProcess)
            {
                _logger.LogWarning("User {UserId} attempted to update loan {LoanId} with status {Status}", userId, loanId, loan.Status);
                throw new BadRequestException("შეგიძლიათ შეცვალოთ მხოლოდ დამუშავების პროცესში მყოფი სესხები");
            }

            if (updateLoanDto.LoanType.HasValue)
                loan.LoanType = updateLoanDto.LoanType.Value;

            if (updateLoanDto.Amount.HasValue)
                loan.Amount = updateLoanDto.Amount.Value;

            if (updateLoanDto.Currency.HasValue)
                loan.Currency = updateLoanDto.Currency.Value;

            if (updateLoanDto.Period.HasValue)
                loan.Period = updateLoanDto.Period.Value;

            var updatedLoan = await _loanRepository.UpdateAsync(loan);
            _logger.LogInformation("Loan {LoanId} updated by user {UserId}", loanId, userId);

            return MapToLoanDto(updatedLoan);
        }

        public async Task DeleteLoanAsync(int userId, int loanId)
        {
            var loan = await _loanRepository.GetByIdAsync(loanId);
            if (loan == null)
            {
                throw new NotFoundException("სესხი ვერ მოიძებნა");
            }

            if (loan.UserId != userId)
            {
                _logger.LogWarning("User {UserId} attempted to delete loan {LoanId} of another user", userId, loanId);
                throw new ForbiddenException("თქვენ არ გაქვთ წვდომა ამ სესხზე");
            }

            if (loan.Status != LoanStatus.InProcess)
            {
                _logger.LogWarning("User {UserId} attempted to delete loan {LoanId} with status {Status}", userId, loanId, loan.Status);
                throw new BadRequestException("შეგიძლიათ წაშალოთ მხოლოდ დამუშავების პროცესში მყოფი სესხები");
            }

            await _loanRepository.DeleteAsync(loanId);
            _logger.LogInformation("Loan {LoanId} deleted by user {UserId}", loanId, userId);
        }

        // Accountant methods
        public async Task<IEnumerable<LoanDto>> GetAllLoansAsync()
        {
            var loans = await _loanRepository.GetAllAsync();
            return loans.Select(MapToLoanDto);
        }

        public async Task<LoanDto> GetAnyLoanByIdAsync(int loanId)
        {
            var loan = await _loanRepository.GetByIdAsync(loanId);
            if (loan == null)
            {
                throw new NotFoundException("სესხი ვერ მოიძებნა");
            }

            return MapToLoanDto(loan);
        }

        public async Task<LoanDto> UpdateAnyLoanAsync(int loanId, UpdateLoanDto updateLoanDto)
        {
            var loan = await _loanRepository.GetByIdAsync(loanId);
            if (loan == null)
            {
                throw new NotFoundException("სესხი ვერ მოიძებნა");
            }

            if (updateLoanDto.LoanType.HasValue)
                loan.LoanType = updateLoanDto.LoanType.Value;

            if (updateLoanDto.Amount.HasValue)
                loan.Amount = updateLoanDto.Amount.Value;

            if (updateLoanDto.Currency.HasValue)
                loan.Currency = updateLoanDto.Currency.Value;

            if (updateLoanDto.Period.HasValue)
                loan.Period = updateLoanDto.Period.Value;

            var updatedLoan = await _loanRepository.UpdateAsync(loan);
            _logger.LogInformation("Loan {LoanId} updated by accountant", loanId);

            return MapToLoanDto(updatedLoan);
        }

        public async Task<LoanDto> UpdateLoanStatusAsync(int loanId, LoanStatus status)
        {
            var loan = await _loanRepository.GetByIdAsync(loanId);
            if (loan == null)
            {
                throw new NotFoundException("სესხი ვერ მოიძებნა");
            }

            loan.Status = status;
            var updatedLoan = await _loanRepository.UpdateAsync(loan);
            _logger.LogInformation("Loan {LoanId} status updated to {Status} by accountant", loanId, status);

            return MapToLoanDto(updatedLoan);
        }

        public async Task DeleteAnyLoanAsync(int loanId)
        {
            var exists = await _loanRepository.ExistsAsync(loanId);
            if (!exists)
            {
                throw new NotFoundException("სესხი ვერ მოიძებნა");
            }

            await _loanRepository.DeleteAsync(loanId);
            _logger.LogInformation("Loan {LoanId} deleted by accountant", loanId);
        }

        private static LoanDto MapToLoanDto(Loan loan)
        {
            return new LoanDto
            {
                Id = loan.Id,
                LoanType = loan.LoanType,
                Amount = loan.Amount,
                Currency = loan.Currency,
                Period = loan.Period,
                Status = loan.Status,
                CreatedAt = loan.CreatedAt,
                UpdatedAt = loan.UpdatedAt,
                UserId = loan.UserId
            };
        }
    }
}
