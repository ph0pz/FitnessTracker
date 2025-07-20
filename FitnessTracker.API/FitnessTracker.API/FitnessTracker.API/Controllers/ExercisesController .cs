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
    public class ExercisesController : ControllerBase
    {
        private readonly IExerciseService _exerciseService;

        public ExercisesController(IExerciseService exerciseService)
        {
            _exerciseService = exerciseService;
        }

        [HttpPost]
        public async Task<IActionResult> AddExercise([FromBody] AddExerciseRequestDto request)
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

            var exerciseEntry = await _exerciseService.AddExerciseAsync(parsedUserId, request);
            return CreatedAtAction(nameof(GetExerciseById), new { id = exerciseEntry?.Id }, exerciseEntry);
        }

        [HttpGet("{date}")] // e.g., /api/exercises/2025-07-20
        public async Task<IActionResult> GetExercisesByDate(string date)
        {
            if (!DateTime.TryParse(date, out var parsedDate))
            {
                return BadRequest("Invalid date format. Use YYYY-MM-DD.");
            }

            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (userId == null || !int.TryParse(userId, out var parsedUserId))
            {
                return Unauthorized("Invalid user token.");
            }

            var exercises = await _exerciseService.GetExercisesByDateAsync(parsedUserId, parsedDate);
            return Ok(exercises);
        }

        [HttpGet("{id:int}", Name = "GetExerciseById")]
        public async Task<IActionResult> GetExerciseById(int id)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (userId == null || !int.TryParse(userId, out var parsedUserId))
            {
                return Unauthorized("Invalid user token.");
            }

            // A dedicated GetExerciseByIdAsync in service would be better
            var exercise = (await _exerciseService.GetExercisesByDateAsync(parsedUserId, DateTime.Today)) // Placeholder
                           .FirstOrDefault(e => e.Id == id);

            if (exercise == null)
            {
                return NotFound("Exercise not found.");
            }
            return Ok(exercise);
        }


        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateExercise(int id, [FromBody] AddExerciseRequestDto request)
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

            var updatedExercise = await _exerciseService.UpdateExerciseAsync(parsedUserId, id, request);
            if (updatedExercise == null)
            {
                return NotFound("Exercise not found or not authorized.");
            }

            return Ok(updatedExercise);
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteExercise(int id)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (userId == null || !int.TryParse(userId, out var parsedUserId))
            {
                return Unauthorized("Invalid user token.");
            }

            var result = await _exerciseService.DeleteExerciseAsync(parsedUserId, id);
            if (!result)
            {
                return NotFound("Exercise not found or not authorized.");
            }

            return NoContent();
        }

        [HttpPut("{id}/mark-done")]
        public async Task<IActionResult> MarkExerciseAsDone(int id, [FromQuery] bool isCompleted = true)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (userId == null || !int.TryParse(userId, out var parsedUserId))
            {
                return Unauthorized("Invalid user token.");
            }

            var result = await _exerciseService.MarkExerciseAsCompletedAsync(parsedUserId, id, isCompleted);
            if (!result)
            {
                return NotFound("Exercise not found or not authorized.");
            }

            return Ok($"Exercise marked as {(isCompleted ? "completed" : "incomplete")}.");
        }
    }
}
