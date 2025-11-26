using Loan_API_project.Exceptions;
using Loan_API_project.Helpers;
using Loan_API_project.Models.DTO;
using Loan_API_project.Models.Entities;
using Loan_API_project.Repositories;
using System.IdentityModel;

namespace Loan_API_project.Services
{
    public interface IAuthService
    {
        Task<LoginResponseDto> RegisterAsync(RegisterDto registerDto);
        Task<LoginResponseDto> LoginAsync(LoginDto loginDto);
    }
    public class AuthService : IAuthService
    {
        private readonly IUserRepository _userRepository;
        private readonly IJwtHelper _jwtHelper;
        private readonly ILogger<AuthService> _logger;

        public AuthService(IUserRepository userRepository, IJwtHelper jwtHelper, ILogger<AuthService> logger)
        {
            _userRepository = userRepository;
            _jwtHelper = jwtHelper;
            _logger = logger;
        }

        public async Task<LoginResponseDto> RegisterAsync(RegisterDto registerDto)
        {
            // Check if username exists
            var existingUser = await _userRepository.GetByUsernameAsync(registerDto.Username);
            if (existingUser != null)
            {
                _logger.LogWarning("Registration failed: Username {Username} already exists", registerDto.Username);
                throw new BadRequestException("მომხმარებლის სახელი უკვე დაკავებულია");
            }

            // Check if email exists
            var existingEmail = await _userRepository.GetByEmailAsync(registerDto.Email);
            if (existingEmail != null)
            {
                _logger.LogWarning("Registration failed: Email {Email} already exists", registerDto.Email);
                throw new BadRequestException("ელ. ფოსტა უკვე გამოყენებულია");
            }

            // Create user
            var user = new User
            {
                FirstName = registerDto.FirstName,
                LastName = registerDto.LastName,
                Username = registerDto.Username,
                Age = registerDto.Age,
                Email = registerDto.Email,
                MonthlyIncome = registerDto.MonthlyIncome,
                PasswordHash = BCrypt.Net.BCrypt.HashPassword(registerDto.Password),
                Role = "User",
                IsBlocked = false
            };

            var createdUser = await _userRepository.CreateAsync(user);
            _logger.LogInformation("User {Username} registered successfully", createdUser.Username);

            var token = _jwtHelper.GenerateToken(createdUser);

            return new LoginResponseDto
            {
                Token = token,
                User = MapToUserDto(createdUser)
            };
        }

        public async Task<LoginResponseDto> LoginAsync(LoginDto loginDto)
        {
            var user = await _userRepository.GetByUsernameAsync(loginDto.Username);

            if (user == null || !BCrypt.Net.BCrypt.Verify(loginDto.Password, user.PasswordHash))
            {
                _logger.LogWarning("Login failed for username: {Username}", loginDto.Username);
                throw new UnauthorizedException("არასწორი მომხმარებლის სახელი ან პაროლი");
            }

            _logger.LogInformation("User {Username} logged in successfully", user.Username);

            var token = _jwtHelper.GenerateToken(user);

            return new LoginResponseDto
            {
                Token = token,
                User = MapToUserDto(user)
            };
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
