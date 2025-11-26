using System.Security.Claims;
using Loan_API_project.Models.DTO;
using Loan_API_project.Models.Entities;
using Loan_API_project.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Loan_API_project.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize(Roles = "User")]
    public class LoanController : ControllerBase
    {
        private readonly ILoanService _loanService;

        public LoanController(ILoanService loanService)
        {
            _loanService = loanService;
        }

        /// <summary>
        /// Create a new loan application
        /// </summary>
        [HttpPost]
        [ProducesResponseType(typeof(ApiResponse<LoanDto>), StatusCodes.Status201Created)]
        [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status403Forbidden)]
        public async Task<IActionResult> CreateLoan([FromBody] CreateLoanDto createLoanDto)
        {
            var userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)!.Value);
            var loan = await _loanService.CreateLoanAsync(userId, createLoanDto);

            return CreatedAtAction(nameof(GetLoanById), new { id = loan.Id }, new ApiResponse<LoanDto>
            {
                Success = true,
                Message = "სესხის განაცხადი წარმატებით შეიქმნა",
                Data = loan
            });
        }

        /// <summary>
        /// Get all loans for the current user
        /// </summary>
        [HttpGet]
        [ProducesResponseType(typeof(ApiResponse<IEnumerable<LoanDto>>), StatusCodes.Status200OK)]
        public async Task<IActionResult> GetUserLoans()
        {
            var userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)!.Value);
            var loans = await _loanService.GetUserLoansAsync(userId);

            return Ok(new ApiResponse<IEnumerable<LoanDto>>
            {
                Success = true,
                Message = "თქვენი სესხები",
                Data = loans
            });
        }

        /// <summary>
        /// Get a specific loan by ID (only user's own loans)
        /// </summary>
        [HttpGet("{id}")]
        [ProducesResponseType(typeof(ApiResponse<LoanDto>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status403Forbidden)]
        public async Task<IActionResult> GetLoanById(int id)
        {
            var userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)!.Value);
            var loan = await _loanService.GetLoanByIdAsync(userId, id);

            return Ok(new ApiResponse<LoanDto>
            {
                Success = true,
                Message = "სესხის ინფორმაცია",
                Data = loan
            });
        }

        /// <summary>
        /// Update a loan (only if status is InProcess)
        /// </summary>
        [HttpPut("{id}")]
        [ProducesResponseType(typeof(ApiResponse<LoanDto>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status403Forbidden)]
        [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status404NotFound)]
        public async Task<IActionResult> UpdateLoan(int id, [FromBody] UpdateLoanDto updateLoanDto)
        {
            var userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)!.Value);
            var loan = await _loanService.UpdateLoanAsync(userId, id, updateLoanDto);

            return Ok(new ApiResponse<LoanDto>
            {
                Success = true,
                Message = "სესხი წარმატებით განახლდა",
                Data = loan
            });
        }

        /// <summary>
        /// Delete a loan (only if status is InProcess)
        /// </summary>
        [HttpDelete("{id}")]
        [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status403Forbidden)]
        [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status404NotFound)]
        public async Task<IActionResult> DeleteLoan(int id)
        {
            var userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)!.Value);
            await _loanService.DeleteLoanAsync(userId, id);

            return Ok(new ApiResponse<object>
            {
                Success = true,
                Message = "სესხი წარმატებით წაიშალა",
                Data = null
            });
        }
    }
}
