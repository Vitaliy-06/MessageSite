using MessageSiteAPI.Models.Dtos;
using MessageSiteAPI.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace MessageSiteAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : Controller
    {
        private readonly IAuthService _authService;
        private readonly ILogger<AuthController> _logger;

        public AuthController(IAuthService authService, ILogger<AuthController> logger)
        {
            _authService = authService;
            _logger = logger;
        }

        [HttpPost("register")]
        public async Task<IActionResult> Register([FromBody] RegLogUserDto body)
        {
            var result = await _authService.RegisterAsync(body.email, body.password);

            if (result.Errors.Any())
            {
                return StatusCode(StatusCodes.Status400BadRequest, new
                {
                    message = "Registration failed",
                    errors = result.Errors.Select(e => e.Description)
                });
            }

            return StatusCode(StatusCodes.Status201Created, new { message = "Registration success" });
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] RegLogUserDto body)
        {
            var result = await _authService.LoginAsync(body.email, body.password);

            if (!result.Succeeded)
            {
                return StatusCode(StatusCodes.Status401Unauthorized, new { message = "Login failed" });
            }

            var user = await _authService.GetUserByEmailAsync(body.email);
            var jwtToken = _authService.GenerateJwtToken(user);
            return StatusCode(StatusCodes.Status200OK, new { message = "Login success", token = jwtToken });
        }

        [HttpGet("confirm-email")]
        public async Task<IActionResult> ConfirmEmail(string userId, string token)
        {
            var result = await _authService.ConfirmEmailAsync(userId, token);

            if (!result.Succeeded)
            {
                return StatusCode(StatusCodes.Status400BadRequest, new
                {
                    message = "Email confirmation failed",
                    errors = result.Errors.Select(e => e.Description)
                });
            }

            return Content("<h2>Email confirmed</h2>", "text/html");
        }

        [HttpGet("get-user")]
        [Authorize]
        public async Task<IActionResult> ProtectedRoute()
        {
            var userEmail = User.FindFirst(ClaimTypes.Email)?.Value;

            var user = await _authService.GetUserByEmailAsync(userEmail);
            if(user == null)
            {
                return StatusCode(StatusCodes.Status404NotFound, new { message = "User not found" });
            }

            return StatusCode(StatusCodes.Status200OK, new
            {
                message = "This is a protected route. You are authorized to access it.",
                user = new
                {
                    email = user.Email,
                    username = user.UserName,
                }
            });
        }
    }
}
