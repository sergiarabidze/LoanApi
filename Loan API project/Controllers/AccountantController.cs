using Loan_API_project.Models.DTO;
using Loan_API_project.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Loan_API_project.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize(Roles = "Accountant")]
    public class AccountantController : ControllerBase
    {
        private readonly ILoanService _loanService;
        private readonly IUserService _userService;

        public AccountantController(ILoanService loanService, IUserService userService)
        {
            _loanService = loanService;
            _userService = userService;
        }

        // ============ Loan Management ============

        /// <summary>
        /// Get all loans (Accountant only)
        /// </summary>
        [HttpGet("loans")]
        [ProducesResponseType(typeof(ApiResponse<IEnumerable<LoanDto>>), StatusCodes.Status200OK)]
        public async Task<IActionResult> GetAllLoans()
        {
            var loans = await _loanService.GetAllLoansAsync();

            return Ok(new ApiResponse<IEnumerable<LoanDto>>
            {
                Success = true,
                Message = "ყველა სესხი",
                Data = loans
            });
        }

        /// <summary>
        /// Get any loan by ID (Accountant only)
        /// </summary>
        [HttpGet("loans/{id}")]
        [ProducesResponseType(typeof(ApiResponse<LoanDto>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status404NotFound)]
        public async Task<IActionResult> GetLoanById(int id)
        {
            var loan = await _loanService.GetAnyLoanByIdAsync(id);

            return Ok(new ApiResponse<LoanDto>
            {
                Success = true,
                Message = "სესხის ინფორმაცია",
                Data = loan
            });
        }

        /// <summary>
        /// Update any loan (Accountant only)
        /// </summary>
        [HttpPut("loans/{id}")]
        [ProducesResponseType(typeof(ApiResponse<LoanDto>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status404NotFound)]
        public async Task<IActionResult> UpdateLoan(int id, [FromBody] UpdateLoanDto updateLoanDto)
        {
            var loan = await _loanService.UpdateAnyLoanAsync(id, updateLoanDto);

            return Ok(new ApiResponse<LoanDto>
            {
                Success = true,
                Message = "სესხი წარმატებით განახლდა",
                Data = loan
            });
        }

        /// <summary>
        /// Update loan status (Accountant only)
        /// </summary>
        [HttpPatch("loans/{id}/status")]
        [ProducesResponseType(typeof(ApiResponse<LoanDto>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status404NotFound)]
        public async Task<IActionResult> UpdateLoanStatus(int id, [FromBody] UpdateLoanStatusDto statusDto)
        {
            var loan = await _loanService.UpdateLoanStatusAsync(id, statusDto.Status);

            return Ok(new ApiResponse<LoanDto>
            {
                Success = true,
                Message = "სესხის სტატუსი წარმატებით განახლდა",
                Data = loan
            });
        }

        /// <summary>
        /// Delete any loan (Accountant only)
        /// </summary>
        [HttpDelete("loans/{id}")]
        [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status404NotFound)]
        public async Task<IActionResult> DeleteLoan(int id)
        {
            await _loanService.DeleteAnyLoanAsync(id);

            return Ok(new ApiResponse<object>
            {
                Success = true,
                Message = "სესხი წარმატებით წაიშალა",
                Data = null
            });
        }

        // ============ User Management ============

        /// <summary>
        /// Block a user (Accountant only)
        /// </summary>
        [HttpPut("users/{id}/block")]
        [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status404NotFound)]
        public async Task<IActionResult> BlockUser(int id)
        {
            await _userService.BlockUserAsync(id, true);

            return Ok(new ApiResponse<object>
            {
                Success = true,
                Message = "მომხმარებელი წარმატებით დაიბლოკა",
                Data = null
            });
        }

        /// <summary>
        /// Unblock a user (Accountant only)
        /// </summary>
        [HttpPut("users/{id}/unblock")]
        [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status404NotFound)]
        public async Task<IActionResult> UnblockUser(int id)
        {
            await _userService.BlockUserAsync(id, false);

            return Ok(new ApiResponse<object>
            {
                Success = true,
                Message = "მომხმარებელი წარმატებით განიბლოკა",
                Data = null
            });
        }

        /// <summary>
        /// Toggle user block status (Accountant only)
        /// </summary>
        [HttpPatch("users/{id}/block")]
        [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status404NotFound)]
        public async Task<IActionResult> ToggleUserBlock(int id, [FromBody] BlockUserDto blockDto)
        {
            await _userService.BlockUserAsync(id, blockDto.IsBlocked);

            var message = blockDto.IsBlocked
                ? "მომხმარებელი წარმატებით დაიბლოკა"
                : "მომხმარებელი წარმატებით განიბლოკა";

            return Ok(new ApiResponse<object>
            {
                Success = true,
                Message = message,
                Data = null
            });
        }
    }
}
