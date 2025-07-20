using FitnessTracker.API.DTOs.Request;
using FitnessTracker.API.Interfaces;
using FitnessTracker.API.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace FitnessTracker.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize] // All actions in this controller require authentication
    public class ProfileController : ControllerBase
    {
        private readonly IAuthService _authService; // AuthService handles user profile details now

        public ProfileController(IAuthService authService)
        {
            _authService = authService;
        }

        [HttpGet]
        public async Task<IActionResult> GetUserProfile()
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (userId == null || !int.TryParse(userId, out var parsedUserId))
            {
                return Unauthorized("Invalid user token.");
            }

            var profile = await _authService.GetUserProfileAsync(parsedUserId);
            if (profile == null)
            {
                return NotFound("User profile not found.");
            }

            return Ok(profile);
        }

        [HttpPut]
        public async Task<IActionResult> UpdateUserProfile([FromBody] UpdateUserProfileRequestDto request)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (userId == null || !int.TryParse(userId, out var parsedUserId))
            {
                return Unauthorized("Invalid user token.");
            }

            var updatedProfile = await _authService.UpdateUserProfileAsync(parsedUserId, request);
            if (updatedProfile == null)
            {
                // This could mean not found, or email conflict, service should ideally throw specific exception
                // For simplicity, returning BadRequest for now or a more specific error based on service logic
                return BadRequest("Failed to update profile. Email might be in use or user not found.");
            }

            return Ok(updatedProfile);
        }
    }
}
