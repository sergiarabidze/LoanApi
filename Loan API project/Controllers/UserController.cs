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
    [Authorize]
    public class UserController : ControllerBase
    {
        private readonly IUserService _userService;

        public UserController(IUserService userService)
        {
            _userService = userService;
        }

        /// <summary>
        /// Get current logged-in user information
        /// </summary>
        [HttpGet("me")]
        [ProducesResponseType(typeof(ApiResponse<UserDto>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status401Unauthorized)]
        public async Task<IActionResult> GetCurrentUser()
        {
            var userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)!.Value);
            var user = await _userService.GetCurrentUserAsync(userId);

            return Ok(new ApiResponse<UserDto>
            {
                Success = true,
                Message = "მომხმარებლის ინფორმაცია",
                Data = user
            });
        }

        /// <summary>
        /// Get user by ID (Accessible to authenticated users for their own profile)
        /// </summary>
        [HttpGet("{id}")]
        [ProducesResponseType(typeof(ApiResponse<UserDto>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status404NotFound)]
        public async Task<IActionResult> GetById(int id)
        {
            var currentUserId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)!.Value);
            var currentUserRole = User.FindFirst(ClaimTypes.Role)!.Value;

            // Only accountants can view other users' information
            if (currentUserId != id && currentUserRole != "Accountant")
            {
                return Forbid();
            }

            var user = await _userService.GetByIdAsync(id);

            return Ok(new ApiResponse<UserDto>
            {
                Success = true,
                Message = "მომხმარებლის ინფორმაცია",
                Data = user
            });
        }
    }
}
