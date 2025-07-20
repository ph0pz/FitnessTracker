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
    [Authorize]
    public class MacroGoalsController : ControllerBase
    {
        private readonly IMacroGoalService _macroGoalService;

        public MacroGoalsController(IMacroGoalService macroGoalService)
        {
            _macroGoalService = macroGoalService;
        }

        [HttpGet("current")]
        public async Task<IActionResult> GetCurrentMacroGoal()
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (userId == null || !int.TryParse(userId, out var parsedUserId))
            {
                return Unauthorized("Invalid user token.");
            }

            var goal = await _macroGoalService.GetCurrentMacroGoalAsync(parsedUserId);
            if (goal == null)
            {
                return NotFound("No active macro goal found for this user.");
            }

            return Ok(goal);
        }

        [HttpPost] // Changed to POST as it creates a new goal and deactivates old ones
        public async Task<IActionResult> SetMacroGoal([FromBody] SetMacroGoalRequestDto request)
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

            var newGoal = await _macroGoalService.SetMacroGoalAsync(parsedUserId, request);
            return CreatedAtAction(nameof(GetCurrentMacroGoal), newGoal); // Return the newly set goal
        }

        [HttpGet("history")]
        public async Task<IActionResult> GetMacroGoalHistory()
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (userId == null || !int.TryParse(userId, out var parsedUserId))
            {
                return Unauthorized("Invalid user token.");
            }

            var history = await _macroGoalService.GetMacroGoalHistoryAsync(parsedUserId);
            return Ok(history);
        }
    }
}
