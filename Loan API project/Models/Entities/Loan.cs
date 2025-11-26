using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Loan_API_project.Enum;

namespace Loan_API_project.Models.Entities
{
    public class Loan
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public LoanType LoanType { get; set; }

        [Required]
        [Range(0.01, double.MaxValue)]
        public decimal Amount { get; set; }

        [Required]
        public Currency Currency { get; set; }

        [Required]
        [Range(1, 360)] // 1-360 თვე
        public int Period { get; set; }

        [Required]
        public LoanStatus Status { get; set; } = LoanStatus.InProcess;

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        public DateTime? UpdatedAt { get; set; }

        // Foreign key
        [Required]
        public int UserId { get; set; }

        // Navigation property
        [ForeignKey("UserId")]
        public virtual User User { get; set; } = null!;
    }
}
