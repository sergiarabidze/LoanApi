using FluentAssertions;
using Loan_API_project.Data;
using Loan_API_project.Enum;
using Loan_API_project.Models.Entities;
using Loan_API_project.Repositories;
using Microsoft.EntityFrameworkCore;
using Xunit;

namespace LoanApi.Tests
{
    public class UserRepositoryTests
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

        [Fact]
        public async Task GetByIdAsync_ShouldReturnUser_WhenUserExists()
        {
            // Arrange
            var context = GetInMemoryContext();
            var repository = new UserRepository(context);

            var user = new User
            {
                FirstName = "Giorgi",
                LastName = "Meladze",
                Username = "gmeladze",
                Age = 25,
                Email = "giorgi@test.com",
                MonthlyIncome = 2000,
                PasswordHash = "hashedpassword",
                Role = "User"
            };

            await context.Users.AddAsync(user);
            await context.SaveChangesAsync();

            // Act
            var result = await repository.GetByIdAsync(user.Id);

            // Assert
            result.Should().NotBeNull();
            result!.Username.Should().Be("gmeladze");
            result.Email.Should().Be("giorgi@test.com");
            result.MonthlyIncome.Should().Be(2000);
        }

        [Fact]
        public async Task GetByIdAsync_ShouldReturnNull_WhenUserDoesNotExist()
        {
            // Arrange
            var context = GetInMemoryContext();
            var repository = new UserRepository(context);

            // Act
            var result = await repository.GetByIdAsync(999);

            // Assert
            result.Should().BeNull();
        }

        [Fact]
        public async Task GetByUsernameAsync_ShouldReturnUser_WhenUsernameExists()
        {
            // Arrange
            var context = GetInMemoryContext();
            var repository = new UserRepository(context);

            var user = new User
            {
                FirstName = "Test",
                LastName = "User",
                Username = "testuser",
                Age = 30,
                Email = "test@test.com",
                MonthlyIncome = 1500,
                PasswordHash = "hash",
                Role = "User"
            };

            await context.Users.AddAsync(user);
            await context.SaveChangesAsync();

            // Act
            var result = await repository.GetByUsernameAsync("testuser");

            // Assert
            result.Should().NotBeNull();
            result!.Username.Should().Be("testuser");
        }

        [Fact]
        public async Task GetByUsernameAsync_ShouldReturnNull_WhenUsernameDoesNotExist()
        {
            // Arrange
            var context = GetInMemoryContext();
            var repository = new UserRepository(context);

            // Act
            var result = await repository.GetByUsernameAsync("nonexistent");

            // Assert
            result.Should().BeNull();
        }

        [Fact]
        public async Task GetByEmailAsync_ShouldReturnUser_WhenEmailExists()
        {
            // Arrange
            var context = GetInMemoryContext();
            var repository = new UserRepository(context);

            var user = new User
            {
                FirstName = "Email",
                LastName = "Test",
                Username = "emailtest",
                Age = 28,
                Email = "unique@test.com",
                MonthlyIncome = 3000,
                PasswordHash = "hash",
                Role = "User"
            };

            await context.Users.AddAsync(user);
            await context.SaveChangesAsync();

            // Act
            var result = await repository.GetByEmailAsync("unique@test.com");

            // Assert
            result.Should().NotBeNull();
            result!.Email.Should().Be("unique@test.com");
        }

        [Fact]
        public async Task CreateAsync_ShouldAddUserToDatabase()
        {
            // Arrange
            var context = GetInMemoryContext();
            var repository = new UserRepository(context);

            var user = new User
            {
                FirstName = "New",
                LastName = "User",
                Username = "newuser",
                Age = 22,
                Email = "new@test.com",
                MonthlyIncome = 1800,
                PasswordHash = "hash",
                Role = "User"
            };

            // Act
            var result = await repository.CreateAsync(user);

            // Assert
            result.Should().NotBeNull();
            result.Id.Should().BeGreaterThan(0);

            var userInDb = await context.Users.FindAsync(result.Id);
            userInDb.Should().NotBeNull();
            userInDb!.Username.Should().Be("newuser");
        }

        [Fact]
        public async Task UpdateAsync_ShouldUpdateUserInDatabase()
        {
            // Arrange
            var context = GetInMemoryContext();
            var repository = new UserRepository(context);

            var user = new User
            {
                FirstName = "Original",
                LastName = "Name",
                Username = "original",
                Age = 25,
                Email = "original@test.com",
                MonthlyIncome = 2000,
                PasswordHash = "hash",
                Role = "User"
            };

            await context.Users.AddAsync(user);
            await context.SaveChangesAsync();

            // Act
            user.FirstName = "Updated";
            user.MonthlyIncome = 2500;
            await repository.UpdateAsync(user);

            // Assert
            var updatedUser = await context.Users.FindAsync(user.Id);
            updatedUser.Should().NotBeNull();
            updatedUser!.FirstName.Should().Be("Updated");
            updatedUser.MonthlyIncome.Should().Be(2500);
        }

        [Fact]
        public async Task ExistsAsync_ShouldReturnTrue_WhenUserExists()
        {
            // Arrange
            var context = GetInMemoryContext();
            var repository = new UserRepository(context);

            var user = new User
            {
                FirstName = "Exists",
                LastName = "User",
                Username = "exists",
                Age = 30,
                Email = "exists@test.com",
                MonthlyIncome = 2000,
                PasswordHash = "hash",
                Role = "User"
            };

            await context.Users.AddAsync(user);
            await context.SaveChangesAsync();

            // Act
            var result = await repository.ExistsAsync(user.Id);

            // Assert
            result.Should().BeTrue();
        }

        [Fact]
        public async Task ExistsAsync_ShouldReturnFalse_WhenUserDoesNotExist()
        {
            // Arrange
            var context = GetInMemoryContext();
            var repository = new UserRepository(context);

            // Act
            var result = await repository.ExistsAsync(999);

            // Assert
            result.Should().BeFalse();
        }
    }

    public class LoanRepositoryTests
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

        [Fact]
        public async Task CreateAsync_ShouldAddLoanToDatabase()
        {
            // Arrange
            var context = GetInMemoryContext();
            var repository = new LoanRepository(context);

            var user = new User
            {
                FirstName = "Test",
                LastName = "User",
                Username = "testuser",
                Age = 25,
                Email = "test@test.com",
                MonthlyIncome = 2000,
                PasswordHash = "hash",
                Role = "User"
            };
            await context.Users.AddAsync(user);
            await context.SaveChangesAsync();

            var loan = new Loan
            {
                UserId = user.Id,
                LoanType = LoanType.QuickLoan,
                Amount = 5000,
                Currency = Currency.GEL,
                Period = 12,
                Status = LoanStatus.InProcess
            };

            // Act
            var result = await repository.CreateAsync(loan);

            // Assert
            result.Should().NotBeNull();
            result.Id.Should().BeGreaterThan(0);
            result.Amount.Should().Be(5000);
            result.Status.Should().Be(LoanStatus.InProcess);
        }

        [Fact]
        public async Task GetByIdAsync_ShouldReturnLoan_WhenLoanExists()
        {
            // Arrange
            var context = GetInMemoryContext();
            var repository = new LoanRepository(context);

            var user = new User
            {
                FirstName = "Test",
                LastName = "User",
                Username = "testuser",
                Age = 25,
                Email = "test@test.com",
                MonthlyIncome = 2000,
                PasswordHash = "hash",
                Role = "User"
            };
            await context.Users.AddAsync(user);
            await context.SaveChangesAsync();

            var loan = new Loan
            {
                UserId = user.Id,
                LoanType = LoanType.AutoLoan,
                Amount = 10000,
                Currency = Currency.USD,
                Period = 24,
                Status = LoanStatus.InProcess
            };
            await context.Loans.AddAsync(loan);
            await context.SaveChangesAsync();

            // Act
            var result = await repository.GetByIdAsync(loan.Id);

            // Assert
            result.Should().NotBeNull();
            result!.Amount.Should().Be(10000);
            result.LoanType.Should().Be(LoanType.AutoLoan);
        }

        [Fact]
        public async Task GetByUserIdAsync_ShouldReturnUserLoans()
        {
            // Arrange
            var context = GetInMemoryContext();
            var repository = new LoanRepository(context);

            var user = new User
            {
                FirstName = "Test",
                LastName = "User",
                Username = "testuser",
                Age = 25,
                Email = "test@test.com",
                MonthlyIncome = 2000,
                PasswordHash = "hash",
                Role = "User"
            };
            await context.Users.AddAsync(user);
            await context.SaveChangesAsync();

            var loan1 = new Loan { UserId = user.Id, LoanType = LoanType.QuickLoan, Amount = 1000, Currency = Currency.GEL, Period = 6, Status = LoanStatus.InProcess };
            var loan2 = new Loan { UserId = user.Id, LoanType = LoanType.AutoLoan, Amount = 2000, Currency = Currency.USD, Period = 12, Status = LoanStatus.Approved };

            await context.Loans.AddRangeAsync(loan1, loan2);
            await context.SaveChangesAsync();

            // Act
            var result = await repository.GetByUserIdAsync(user.Id);

            // Assert
            result.Should().HaveCount(2);
            result.Should().Contain(l => l.Amount == 1000);
            result.Should().Contain(l => l.Amount == 2000);
        }

        [Fact]
        public async Task GetAllAsync_ShouldReturnAllLoans()
        {
            // Arrange
            var context = GetInMemoryContext();
            var repository = new LoanRepository(context);

            var user1 = new User { FirstName = "User1", LastName = "Test", Username = "user1", Age = 25, Email = "user1@test.com", MonthlyIncome = 2000, PasswordHash = "hash", Role = "User" };
            var user2 = new User { FirstName = "User2", LastName = "Test", Username = "user2", Age = 30, Email = "user2@test.com", MonthlyIncome = 3000, PasswordHash = "hash", Role = "User" };

            await context.Users.AddRangeAsync(user1, user2);
            await context.SaveChangesAsync();

            var loan1 = new Loan { UserId = user1.Id, LoanType = LoanType.QuickLoan, Amount = 1000, Currency = Currency.GEL, Period = 6, Status = LoanStatus.InProcess };
            var loan2 = new Loan { UserId = user2.Id, LoanType = LoanType.Installment, Amount = 3000, Currency = Currency.EUR, Period = 18, Status = LoanStatus.Approved };

            await context.Loans.AddRangeAsync(loan1, loan2);
            await context.SaveChangesAsync();

            // Act
            var result = await repository.GetAllAsync();

            // Assert
            result.Should().HaveCount(2);
        }

        [Fact]
        public async Task UpdateAsync_ShouldUpdateLoan()
        {
            // Arrange
            var context = GetInMemoryContext();
            var repository = new LoanRepository(context);

            var user = new User
            {
                FirstName = "Test",
                LastName = "User",
                Username = "testuser",
                Age = 25,
                Email = "test@test.com",
                MonthlyIncome = 2000,
                PasswordHash = "hash",
                Role = "User"
            };
            await context.Users.AddAsync(user);
            await context.SaveChangesAsync();

            var loan = new Loan
            {
                UserId = user.Id,
                LoanType = LoanType.QuickLoan,
                Amount = 5000,
                Currency = Currency.GEL,
                Period = 12,
                Status = LoanStatus.InProcess
            };
            await context.Loans.AddAsync(loan);
            await context.SaveChangesAsync();

            // Act
            loan.Amount = 6000;
            loan.Status = LoanStatus.Approved;
            await repository.UpdateAsync(loan);

            // Assert
            var updatedLoan = await context.Loans.FindAsync(loan.Id);
            updatedLoan.Should().NotBeNull();
            updatedLoan!.Amount.Should().Be(6000);
            updatedLoan.Status.Should().Be(LoanStatus.Approved);
            updatedLoan.UpdatedAt.Should().NotBeNull();
        }

        [Fact]
        public async Task DeleteAsync_ShouldRemoveLoan()
        {
            // Arrange
            var context = GetInMemoryContext();
            var repository = new LoanRepository(context);

            var user = new User
            {
                FirstName = "Test",
                LastName = "User",
                Username = "testuser",
                Age = 25,
                Email = "test@test.com",
                MonthlyIncome = 2000,
                PasswordHash = "hash",
                Role = "User"
            };
            await context.Users.AddAsync(user);
            await context.SaveChangesAsync();

            var loan = new Loan
            {
                UserId = user.Id,
                LoanType = LoanType.QuickLoan,
                Amount = 5000,
                Currency = Currency.GEL,
                Period = 12,
                Status = LoanStatus.InProcess
            };
            await context.Loans.AddAsync(loan);
            await context.SaveChangesAsync();

            var loanId = loan.Id;

            // Act
            await repository.DeleteAsync(loanId);

            // Assert
            var deletedLoan = await context.Loans.FindAsync(loanId);
            deletedLoan.Should().BeNull();
        }

        [Fact]
        public async Task ExistsAsync_ShouldReturnTrue_WhenLoanExists()
        {
            // Arrange
            var context = GetInMemoryContext();
            var repository = new LoanRepository(context);

            var user = new User
            {
                FirstName = "Test",
                LastName = "User",
                Username = "testuser",
                Age = 25,
                Email = "test@test.com",
                MonthlyIncome = 2000,
                PasswordHash = "hash",
                Role = "User"
            };
            await context.Users.AddAsync(user);
            await context.SaveChangesAsync();

            var loan = new Loan
            {
                UserId = user.Id,
                LoanType = LoanType.QuickLoan,
                Amount = 5000,
                Currency = Currency.GEL,
                Period = 12,
                Status = LoanStatus.InProcess
            };
            await context.Loans.AddAsync(loan);
            await context.SaveChangesAsync();

            // Act
            var result = await repository.ExistsAsync(loan.Id);

            // Assert
            result.Should().BeTrue();
        }

        [Fact]
        public async Task ExistsAsync_ShouldReturnFalse_WhenLoanDoesNotExist()
        {
            // Arrange
            var context = GetInMemoryContext();
            var repository = new LoanRepository(context);

            // Act
            var result = await repository.ExistsAsync(999);

            // Assert
            result.Should().BeFalse();
        }
    }
}