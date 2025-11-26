using Loan_API_project.Exceptions;
using Loan_API_project.Models.DTO;
using Loan_API_project.Models.Entities;
using Loan_API_project.Repositories;

namespace Loan_API_project.Services
{
    public interface IUserService
    {
        Task<UserDto> GetByIdAsync(int id);
        Task<UserDto> GetCurrentUserAsync(int userId);
        Task BlockUserAsync(int userId, bool isBlocked);
    }
    public class UserService : IUserService
    {
        private readonly IUserRepository _userRepository;
        private readonly ILogger<UserService> _logger;

        public UserService(IUserRepository userRepository, ILogger<UserService> logger)
        {
            _userRepository = userRepository;
            _logger = logger;
        }

        public async Task<UserDto> GetByIdAsync(int id)
        {
            var user = await _userRepository.GetByIdAsync(id);
            if (user == null)
            {
                _logger.LogWarning("User with id {UserId} not found", id);
                throw new NotFoundException("მომხმარებელი ვერ მოიძებნა");
            }

            return MapToUserDto(user);
        }

        public async Task<UserDto> GetCurrentUserAsync(int userId)
        {
            return await GetByIdAsync(userId);
        }

        public async Task BlockUserAsync(int userId, bool isBlocked)
        {
            var user = await _userRepository.GetByIdAsync(userId);
            if (user == null)
            {
                _logger.LogWarning("User with id {UserId} not found for blocking", userId);
                throw new NotFoundException("მომხმარებელი ვერ მოიძებნა");
            }

            if (user.Role == "Accountant")
            {
                _logger.LogWarning("Attempt to block accountant user {UserId}", userId);
                throw new BadRequestException("ბუღალტერის დაბლოკვა შეუძლებელია");
            }

            user.IsBlocked = isBlocked;
            await _userRepository.UpdateAsync(user);

            _logger.LogInformation("User {UserId} block status changed to {IsBlocked}", userId, isBlocked);
        }

        private static UserDto MapToUserDto(User user)
        {
            return new UserDto
            {
                Id = user.Id,
                FirstName = user.FirstName,
                LastName = user.LastName,
                Username = user.Username,
                Age = user.Age,
                Email = user.Email,
                MonthlyIncome = user.MonthlyIncome,
                IsBlocked = user.IsBlocked,
                Role = user.Role
            };
        }
    }
}
