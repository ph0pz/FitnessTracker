using FitnessTracker.API.DTOs;
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
    public class ProgressController : ControllerBase
    {
        private readonly IProgressService _progressService;

        public ProgressController(IProgressService progressService)
        {
            _progressService = progressService;
        }

        [HttpPost("weight")]
        public async Task<IActionResult> AddWeightLog([FromBody] AddWeightLogRequestDto request)
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

            var weightLog = await _progressService.AddWeightLogAsync(parsedUserId, request);
            return CreatedAtAction(nameof(GetWeightLogById), new { id = weightLog?.Id }, weightLog);
        }

        [HttpGet("weight-history")]
        public async Task<IActionResult> GetWeightHistory()
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (userId == null || !int.TryParse(userId, out var parsedUserId))
            {
                return Unauthorized("Invalid user token.");
            }

            var history = await _progressService.GetWeightHistoryAsync(parsedUserId);
            return Ok(history);
        }

        [HttpGet("weight/{id:int}", Name = "GetWeightLogById")]
        public async Task<IActionResult> GetWeightLogById(int id)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (userId == null || !int.TryParse(userId, out var parsedUserId))
            {
                return Unauthorized("Invalid user token.");
            }

            // A dedicated GetWeightLogByIdAsync in service would be better
            var log = (await _progressService.GetWeightHistoryAsync(parsedUserId)) // Placeholder
                       .FirstOrDefault(w => w.Id == id);

            if (log == null)
            {
                return NotFound("Weight log not found.");
            }
            return Ok(log);
        }

        [HttpPut("weight/{id}")]
        public async Task<IActionResult> UpdateWeightLog(int id, [FromBody] UpdateWeightLogRequestDto request)
        {
            if (id != request.Id || !ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (userId == null || !int.TryParse(userId, out var parsedUserId))
            {
                return Unauthorized("Invalid user token.");
            }

            var updatedLog = await _progressService.UpdateWeightLogAsync(parsedUserId, id, request);
            if (updatedLog == null)
            {
                return NotFound("Weight log not found or not authorized.");
            }

            return Ok(updatedLog);
        }

        [HttpDelete("weight/{id}")]
        public async Task<IActionResult> DeleteWeightLog(int id)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (userId == null || !int.TryParse(userId, out var parsedUserId))
            {
                return Unauthorized("Invalid user token.");
            }

            var result = await _progressService.DeleteWeightLogAsync(parsedUserId, id);
            if (!result)
            {
                return NotFound("Weight log not found or not authorized.");
            }

            return NoContent();
        }
    }
}
