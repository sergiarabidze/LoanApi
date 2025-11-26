using Loan_API_project.Models.Entities;
using Microsoft.EntityFrameworkCore;

namespace Loan_API_project.Data
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }

        public DbSet<User> Users { get; set; } = null!;
        public DbSet<Loan> Loans { get; set; } = null!;

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // User configuration
            modelBuilder.Entity<User>(entity =>
            {
                entity.ToTable("Users");
                entity.HasKey(e => e.Id);
                entity.HasIndex(e => e.Username).IsUnique();
                entity.HasIndex(e => e.Email).IsUnique();

                entity.Property(e => e.MonthlyIncome)
                    .HasPrecision(18, 2);

                // One-to-Many relationship
                entity.HasMany(u => u.Loans)
                    .WithOne(l => l.User)
                    .HasForeignKey(l => l.UserId)
                    .OnDelete(DeleteBehavior.Cascade);
            });

            // Loan configuration
            modelBuilder.Entity<Loan>(entity =>
            {
                entity.ToTable("Loans");
                entity.HasKey(e => e.Id);

                entity.Property(e => e.Amount)
                    .HasPrecision(18, 2);

                entity.Property(e => e.LoanType)
                    .HasConversion<int>();

                entity.Property(e => e.Currency)
                    .HasConversion<int>();

                entity.Property(e => e.Status)
                    .HasConversion<int>();
            });

            // Seed default accountant user
            var accountantPasswordHash = BCrypt.Net.BCrypt.HashPassword("Admin123!");

            modelBuilder.Entity<User>().HasData(
                new User
                {
                    Id = 1,
                    FirstName = "Admin",
                    LastName = "Accountant",
                    Username = "accountant",
                    Age = 30,
                    Email = "accountant@loanapi.com",
                    MonthlyIncome = 5000,
                    IsBlocked = false,
                    PasswordHash = accountantPasswordHash,
                    Role = "Accountant",
                    CreatedAt = DateTime.UtcNow
                }
            );
        }
    }
}
