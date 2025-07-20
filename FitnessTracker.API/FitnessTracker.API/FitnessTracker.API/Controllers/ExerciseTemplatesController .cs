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
    public class ExerciseTemplatesController : ControllerBase
    {
        private readonly IExerciseService _exerciseService; // ExerciseService also handles templates

        public ExerciseTemplatesController(IExerciseService exerciseService)
        {
            _exerciseService = exerciseService;
        }

        [HttpPost]
        public async Task<IActionResult> CreateExerciseTemplate([FromBody] CreateExerciseTemplateRequestDto request)
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

            var template = await _exerciseService.CreateExerciseTemplateAsync(parsedUserId, request);
            return CreatedAtAction(nameof(GetExerciseTemplateById), new { id = template?.Id }, template);
        }

        [HttpGet]
        public async Task<IActionResult> GetExerciseTemplates()
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (userId == null || !int.TryParse(userId, out var parsedUserId))
            {
                return Unauthorized("Invalid user token.");
            }

            var templates = await _exerciseService.GetExerciseTemplatesAsync(parsedUserId);
            return Ok(templates);
        }

        [HttpGet("{id:int}", Name = "GetExerciseTemplateById")]
        public async Task<IActionResult> GetExerciseTemplateById(int id)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (userId == null || !int.TryParse(userId, out var parsedUserId))
            {
                return Unauthorized("Invalid user token.");
            }

            var template = await _exerciseService.GetExerciseTemplateByIdAsync(parsedUserId, id);
            if (template == null)
            {
                return NotFound("Exercise template not found or not authorized.");
            }

            return Ok(template);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateExerciseTemplate(int id, [FromBody] UpdateExerciseTemplateRequestDto request)
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

            var updatedTemplate = await _exerciseService.UpdateExerciseTemplateAsync(parsedUserId, id, request);
            if (updatedTemplate == null)
            {
                return NotFound("Exercise template not found or not authorized.");
            }

            return Ok(updatedTemplate);
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteExerciseTemplate(int id)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (userId == null || !int.TryParse(userId, out var parsedUserId))
            {
                return Unauthorized("Invalid user token.");
            }

            var result = await _exerciseService.DeleteExerciseTemplateAsync(parsedUserId, id);
            if (!result)
            {
                return NotFound("Exercise template not found or not authorized.");
            }

            return NoContent();
        }
    }
}
