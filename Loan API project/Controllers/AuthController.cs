using Loan_API_project.Models.DTO;
using Loan_API_project.Services;
using Microsoft.AspNetCore.Mvc;

namespace Loan_API_project.Controllers
{
    [Route("api/[controller]")]
[ApiController]
public class AuthController : ControllerBase
{
    private readonly IAuthService _authService;

    public AuthController(IAuthService authService)
    {
        _authService = authService;
    }

    /// <summary>
    /// Register a new user
    /// </summary>
    [HttpPost("register")]
    [ProducesResponseType(typeof(ApiResponse<LoginResponseDto>), StatusCodes.Status201Created)]
    [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> Register([FromBody] RegisterDto registerDto)
    {
        var result = await _authService.RegisterAsync(registerDto);

        return CreatedAtAction(nameof(Register), new ApiResponse<LoginResponseDto>
        {
            Success = true,
            Message = "რეგისტრაცია წარმატებით დასრულდა",
            Data = result
        });
    }

    /// <summary>
    /// Login with username and password
    /// </summary>
    [HttpPost("login")]
    [ProducesResponseType(typeof(ApiResponse<LoginResponseDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status401Unauthorized)]
    public async Task<IActionResult> Login([FromBody] LoginDto loginDto)
    {
        var result = await _authService.LoginAsync(loginDto);

        return Ok(new ApiResponse<LoginResponseDto>
        {
            Success = true,
            Message = "ავტორიზაცია წარმატებით დასრულდა",
            Data = result
        });
    }
}
}
