using System;
using FluentAssertions;
using Loan_API_project.Data;
using Loan_API_project.Enum;
using Loan_API_project.Helpers;
using Loan_API_project.Models.DTO;
using Loan_API_project.Repositories;
using Loan_API_project.Services;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Moq;
using Xunit;

namespace LoanApi.Tests
{
    /// <summary>
    /// Integration tests that test the full workflow from service to repository to database
    /// </summary>
    public class LoanWorkflowIntegrationTests
    {
        private ApplicationDbContext GetInMemoryContext()
        {
            var options = new DbContextOptionsBuilder<ApplicationDbContext>()
                .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
                .Options;

            var context = new ApplicationDbContext(options);
            context.Database.EnsureCreated();
            return context;
        }

        private IJwtHelper GetMockJwtHelper()
        {
            var configMock = new Mock<IConfiguration>();
            configMock.Setup(x => x["Jwt:Key"]).Returns("YourSuperSecretKeyThatIsAtLeast32CharactersLongForJWT!");
            configMock.Setup(x => x["Jwt:Issuer"]).Returns("LoanApi");
            configMock.Setup(x => x["Jwt:Audience"]).Returns("LoanApiUsers");
            configMock.Setup(x => x["Jwt:ExpiryInHours"]).Returns("24");

            return new JwtHelper(configMock.Object);
        }

        [Fact]
        public async Task CompleteUserJourney_RegisterLoginCreateLoanUpdateLoan_ShouldWork()
        {
            // Arrange
            var context = GetInMemoryContext();
            var userRepository = new UserRepository(context);
            var loanRepository = new LoanRepository(context);
            var jwtHelper = GetMockJwtHelper();
            var authLogger = new Mock<ILogger<AuthService>>().Object;
            var loanLogger = new Mock<ILogger<LoanService>>().Object;

            var authService = new AuthService(userRepository, jwtHelper, authLogger);
            var loanService = new LoanService(loanRepository, userRepository, loanLogger);

            // Step 1: Register a new user
            var registerDto = new RegisterDto
            {
                FirstName = "Giorgi",
                LastName = "Meladze",
                Username = "gmeladze",
                Age = 25,
                Email = "giorgi@test.com",
                MonthlyIncome = 2500,
                Password = "Password123"
            };

            var registerResult = await authService.RegisterAsync(registerDto);

            // Assert registration
            registerResult.Should().NotBeNull();
            registerResult.Token.Should().NotBeNullOrEmpty();
            registerResult.User.Username.Should().Be("gmeladze");
            registerResult.User.IsBlocked.Should().BeFalse();

            var userId = registerResult.User.Id;

            // Step 2: Login with the same credentials
            var loginDto = new LoginDto
            {
                Username = "gmeladze",
                Password = "Password123"
            };

            var loginResult = await authService.LoginAsync(loginDto);

            // Assert login
            loginResult.Should().NotBeNull();
            loginResult.Token.Should().NotBeNullOrEmpty();
            loginResult.User.Id.Should().Be(userId);

            // Step 3: Create a loan
            var createLoanDto = new CreateLoanDto
            {
                LoanType = LoanType.QuickLoan,
                Amount = 5000,
                Currency = Currency.GEL,
                Period = 12
            };

            var createdLoan = await loanService.CreateLoanAsync(userId, createLoanDto);

            // Assert loan creation
            createdLoan.Should().NotBeNull();
            createdLoan.Amount.Should().Be(5000);
            createdLoan.Status.Should().Be(LoanStatus.InProcess);
            createdLoan.UserId.Should().Be(userId);

            // Step 4: Get user's loans
            var userLoans = await loanService.GetUserLoansAsync(userId);

            // Assert loans retrieval
            userLoans.Should().HaveCount(1);
            userLoans.First().Amount.Should().Be(5000);

            // Step 5: Update the loan
            var updateLoanDto = new UpdateLoanDto
            {
                Amount = 6000,
                Period = 18
            };

            var updatedLoan = await loanService.UpdateLoanAsync(userId, createdLoan.Id, updateLoanDto);

            // Assert loan update
            updatedLoan.Amount.Should().Be(6000);
            updatedLoan.Period.Should().Be(18);
            updatedLoan.UpdatedAt.Should().NotBeNull();

            // Step 6: Get specific loan
            var specificLoan = await loanService.GetLoanByIdAsync(userId, createdLoan.Id);

            // Assert specific loan
            specificLoan.Amount.Should().Be(6000);
            specificLoan.Period.Should().Be(18);

            // Step 7: Delete the loan
            await loanService.DeleteLoanAsync(userId, createdLoan.Id);

            // Assert deletion
            var loansAfterDeletion = await loanService.GetUserLoansAsync(userId);
            loansAfterDeletion.Should().BeEmpty();
        }

        [Fact]
        public async Task AccountantWorkflow_ApproveRejectBlockUser_ShouldWork()
        {
            // Arrange
            var context = GetInMemoryContext();
            var userRepository = new UserRepository(context);
            var loanRepository = new LoanRepository(context);
            var jwtHelper = GetMockJwtHelper();
            var authLogger = new Mock<ILogger<AuthService>>().Object;
            var userLogger = new Mock<ILogger<UserService>>().Object;
            var loanLogger = new Mock<ILogger<LoanService>>().Object;

            var authService = new AuthService(userRepository, jwtHelper, authLogger);
            var userService = new UserService(userRepository, userLogger);
            var loanService = new LoanService(loanRepository, userRepository, loanLogger);

            // Step 1: Create a regular user
            var registerDto = new RegisterDto
            {
                FirstName = "Regular",
                LastName = "User",
                Username = "regularuser",
                Age = 30,
                Email = "regular@test.com",
                MonthlyIncome = 2000,
                Password = "Password123"
            };

            var registeredUser = await authService.RegisterAsync(registerDto);
            var userId = registeredUser.User.Id;

            // Step 2: User creates a loan
            var createLoanDto = new CreateLoanDto
            {
                LoanType = LoanType.AutoLoan,
                Amount = 10000,
                Currency = Currency.USD,
                Period = 24
            };

            var loan = await loanService.CreateLoanAsync(userId, createLoanDto);

            // Step 3: Accountant approves the loan
            var approvedLoan = await loanService.UpdateLoanStatusAsync(loan.Id, LoanStatus.Approved);

            // Assert approval
            approvedLoan.Status.Should().Be(LoanStatus.Approved);

            // Step 4: User creates another loan
            var secondLoan = await loanService.CreateLoanAsync(userId, new CreateLoanDto
            {
                LoanType = LoanType.Installment,
                Amount = 3000,
                Currency = Currency.GEL,
                Period = 6
            });

            // Step 5: Accountant rejects the second loan
            var rejectedLoan = await loanService.UpdateLoanStatusAsync(secondLoan.Id, LoanStatus.Rejected);

            // Assert rejection
            rejectedLoan.Status.Should().Be(LoanStatus.Rejected);

            // Step 6: Accountant blocks the user
            await userService.BlockUserAsync(userId, true);

            // Assert user is blocked
            var blockedUser = await userRepository.GetByIdAsync(userId);
            blockedUser!.IsBlocked.Should().BeTrue();

            // Step 7: Blocked user tries to create a loan (should fail)
            var thirdLoanDto = new CreateLoanDto
            {
                LoanType = LoanType.QuickLoan,
                Amount = 1000,
                Currency = Currency.GEL,
                Period = 3
            };

            // Assert blocked user cannot create loan
            await Assert.ThrowsAsync<Loan_API_project.Exceptions.ForbiddenException>(() =>
                loanService.CreateLoanAsync(userId, thirdLoanDto));

            // Step 8: Accountant unblocks the user
            await userService.BlockUserAsync(userId, false);

            // Assert user is unblocked
            var unblockedUser = await userRepository.GetByIdAsync(userId);
            unblockedUser!.IsBlocked.Should().BeFalse();

            // Step 9: Unblocked user can now create loans again
            var fourthLoan = await loanService.CreateLoanAsync(userId, thirdLoanDto);

            // Assert loan creation after unblock
            fourthLoan.Should().NotBeNull();
            fourthLoan.Status.Should().Be(LoanStatus.InProcess);
        }

        [Fact]
        public async Task MultipleUsers_CannotAccessEachOthersLoans()
        {
            // Arrange
            var context = GetInMemoryContext();
            var userRepository = new UserRepository(context);
            var loanRepository = new LoanRepository(context);
            var jwtHelper = GetMockJwtHelper();
            var authLogger = new Mock<ILogger<AuthService>>().Object;
            var loanLogger = new Mock<ILogger<LoanService>>().Object;

            var authService = new AuthService(userRepository, jwtHelper, authLogger);
            var loanService = new LoanService(loanRepository, userRepository, loanLogger);

            // Create User 1
            var user1 = await authService.RegisterAsync(new RegisterDto
            {
                FirstName = "User",
                LastName = "One",
                Username = "user1",
                Age = 25,
                Email = "user1@test.com",
                MonthlyIncome = 2000,
                Password = "Password123"
            });

            // Create User 2
            var user2 = await authService.RegisterAsync(new RegisterDto
            {
                FirstName = "User",
                LastName = "Two",
                Username = "user2",
                Age = 30,
                Email = "user2@test.com",
                MonthlyIncome = 3000,
                Password = "Password123"
            });

            // User 1 creates a loan
            var user1Loan = await loanService.CreateLoanAsync(user1.User.Id, new CreateLoanDto
            {
                LoanType = LoanType.QuickLoan,
                Amount = 5000,
                Currency = Currency.GEL,
                Period = 12
            });

            // User 2 creates a loan
            var user2Loan = await loanService.CreateLoanAsync(user2.User.Id, new CreateLoanDto
            {
                LoanType = LoanType.AutoLoan,
                Amount = 15000,
                Currency = Currency.USD,
                Period = 36
            });

            // User 1 can see only their loans
            var user1Loans = await loanService.GetUserLoansAsync(user1.User.Id);
            user1Loans.Should().HaveCount(1);
            user1Loans.First().Amount.Should().Be(5000);

            // User 2 can see only their loans
            var user2Loans = await loanService.GetUserLoansAsync(user2.User.Id);
            user2Loans.Should().HaveCount(1);
            user2Loans.First().Amount.Should().Be(15000);

            // User 2 cannot access User 1's loan
            await Assert.ThrowsAsync<Loan_API_project.Exceptions.ForbiddenException>(() =>
                loanService.GetLoanByIdAsync(user2.User.Id, user1Loan.Id));

            // User 1 cannot access User 2's loan
            await Assert.ThrowsAsync<Loan_API_project.Exceptions.ForbiddenException>(() =>
                loanService.GetLoanByIdAsync(user1.User.Id, user2Loan.Id));
        }

        [Fact]
        public async Task LoanStatusRestrictions_UserCannotModifyApprovedLoans()
        {
            // Arrange
            var context = GetInMemoryContext();
            var userRepository = new UserRepository(context);
            var loanRepository = new LoanRepository(context);
            var jwtHelper = GetMockJwtHelper();
            var authLogger = new Mock<ILogger<AuthService>>().Object;
            var loanLogger = new Mock<ILogger<LoanService>>().Object;

            var authService = new AuthService(userRepository, jwtHelper, authLogger);
            var loanService = new LoanService(loanRepository, userRepository, loanLogger);

            // Create user
            var user = await authService.RegisterAsync(new RegisterDto
            {
                FirstName = "Test",
                LastName = "User",
                Username = "testuser",
                Age = 28,
                Email = "test@test.com",
                MonthlyIncome = 2500,
                Password = "Password123"
            });

            // Create loan
            var loan = await loanService.CreateLoanAsync(user.User.Id, new CreateLoanDto
            {
                LoanType = LoanType.QuickLoan,
                Amount = 5000,
                Currency = Currency.GEL,
                Period = 12
            });

            // Accountant approves the loan
            await loanService.UpdateLoanStatusAsync(loan.Id, LoanStatus.Approved);

            // User tries to update approved loan (should fail)
            var updateDto = new UpdateLoanDto { Amount = 6000 };
            await Assert.ThrowsAsync<Loan_API_project.Exceptions.BadRequestException>(() =>
                loanService.UpdateLoanAsync(user.User.Id, loan.Id, updateDto));

            // User tries to delete approved loan (should fail)
            await Assert.ThrowsAsync<Loan_API_project.Exceptions.BadRequestException>(() =>
                loanService.DeleteLoanAsync(user.User.Id, loan.Id));
        }

        [Fact]
        public async Task AccountantCanManageAllLoans_RegardlessOfStatus()
        {
            // Arrange
            var context = GetInMemoryContext();
            var userRepository = new UserRepository(context);
            var loanRepository = new LoanRepository(context);
            var jwtHelper = GetMockJwtHelper();
            var authLogger = new Mock<ILogger<AuthService>>().Object;
            var loanLogger = new Mock<ILogger<LoanService>>().Object;

            var authService = new AuthService(userRepository, jwtHelper, authLogger);
            var loanService = new LoanService(loanRepository, userRepository, loanLogger);

            // Create user
            var user = await authService.RegisterAsync(new RegisterDto
            {
                FirstName = "Test",
                LastName = "User",
                Username = "testuser",
                Age = 28,
                Email = "test@test.com",
                MonthlyIncome = 2500,
                Password = "Password123"
            });

            // Create and approve loan
            var loan = await loanService.CreateLoanAsync(user.User.Id, new CreateLoanDto
            {
                LoanType = LoanType.QuickLoan,
                Amount = 5000,
                Currency = Currency.GEL,
                Period = 12
            });

            await loanService.UpdateLoanStatusAsync(loan.Id, LoanStatus.Approved);

            // Accountant can still update approved loan
            var updatedLoan = await loanService.UpdateAnyLoanAsync(loan.Id, new UpdateLoanDto
            {
                Amount = 7000
            });

            updatedLoan.Amount.Should().Be(7000);
            updatedLoan.Status.Should().Be(LoanStatus.Approved);

            // Accountant can delete any loan regardless of status
            await loanService.DeleteAnyLoanAsync(loan.Id);

            // Verify deletion
            var allLoans = await loanService.GetAllLoansAsync();
            allLoans.Should().BeEmpty();
        }

        [Fact]
        public async Task PasswordHashing_ShouldStoreHashedPassword()
        {
            // Arrange
            var context = GetInMemoryContext();
            var userRepository = new UserRepository(context);
            var jwtHelper = GetMockJwtHelper();
            var authLogger = new Mock<ILogger<AuthService>>().Object;

            var authService = new AuthService(userRepository, jwtHelper, authLogger);

            var password = "MySecurePassword123";

            // Act
            var registerDto = new RegisterDto
            {
                FirstName = "Security",
                LastName = "Test",
                Username = "sectest",
                Age = 25,
                Email = "sec@test.com",
                MonthlyIncome = 2000,
                Password = password
            };

            var result = await authService.RegisterAsync(registerDto);

            // Assert
            var userInDb = await userRepository.GetByIdAsync(result.User.Id);
            userInDb.Should().NotBeNull();
            userInDb!.PasswordHash.Should().NotBe(password); // Password should be hashed
            userInDb.PasswordHash.Should().StartWith("$2"); // BCrypt hash prefix

            // Verify password can be validated
            var isValid = BCrypt.Net.BCrypt.Verify(password, userInDb.PasswordHash);
            isValid.Should().BeTrue();
        }
    }
}