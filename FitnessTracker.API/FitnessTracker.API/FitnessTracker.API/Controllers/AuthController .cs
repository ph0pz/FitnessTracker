using FitnessTracker.API.DTOs.Request;
using FitnessTracker.API.DTOs.Request.FitnessTracker.Api.DTOs.Auth;
using FitnessTracker.API.Interfaces;
using FitnessTracker.API.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace FitnessTracker.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AuthController : ControllerBase
    {
        private readonly IAuthService _authService;

        public AuthController(IAuthService authService)
        {
            _authService = authService;
        }

        [HttpPost("register")]
        [AllowAnonymous] // Allow unauthenticated access
        public async Task<IActionResult> Register([FromBody] RegisterRequestDto request)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var result = await _authService.RegisterAsync(request);
            if (!result)
            {
                return Conflict("Username or Email already exists."); // 409 Conflict
            }

            return Ok("Registration successful. Please log in."); // Or 201 Created if desired for a resource
        }

        [HttpPost("login")]
        [AllowAnonymous] // Allow unauthenticated access
        public async Task<IActionResult> Login([FromBody] LoginRequestDto request)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var response = await _authService.LoginAsync(request);
            if (response == null)
            {
                return Unauthorized("Invalid username or password."); // 401 Unauthorized
            }

            return Ok(response); // Returns JWT token and User ID
        }

        [HttpPut("change-password")]
        [Authorize] // Requires authentication
        public async Task<IActionResult> ChangePassword([FromBody] ChangePasswordRequestDto request)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            // Get UserId from JWT claims
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (userId == null || !int.TryParse(userId, out var parsedUserId))
            {
                return Unauthorized("Invalid user token.");
            }

            var result = await _authService.ChangePasswordAsync(parsedUserId, request);
            if (!result)
            {
                return BadRequest("Failed to change password. Check current password or if new passwords match.");
            }

            return Ok("Password changed successfully."); // 200 OK
        }
    }
}
