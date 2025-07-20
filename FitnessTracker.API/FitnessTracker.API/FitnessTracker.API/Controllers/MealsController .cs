
using FitnessTracker.API.DTOs;
using FitnessTracker.API.DTOs.Request;
using FitnessTracker.API.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace FitnessTracker.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class MealsController : ControllerBase
    {
        private readonly IMealService _mealService;

        public MealsController(IMealService mealService)
        {
            _mealService = mealService;
        }

        [HttpPost]
        public async Task<IActionResult> AddMeal([FromBody] AddMealRequestDto request)
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

            var mealEntry = await _mealService.AddMealAsync(parsedUserId, request);
            return CreatedAtAction(nameof(GetMealById), new { id = mealEntry?.Id }, mealEntry);
        }

        [HttpGet("{date}")] // e.g., /api/meals/2025-07-20
        public async Task<IActionResult> GetMealsByDate(string date)
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

            var meals = await _mealService.GetMealsByDateAsync(parsedUserId, parsedDate);
            return Ok(meals);
        }

        [HttpGet("history")] // Optional: for full history with query params for range
        public async Task<IActionResult> GetMealHistory([FromQuery] DateTime? startDate, [FromQuery] DateTime? endDate)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (userId == null || !int.TryParse(userId, out var parsedUserId))
            {
                return Unauthorized("Invalid user token.");
            }
            // For simplicity, service method only takes date, but here you'd call
            // a service method that filters by date range.
            // For now, let's use GetMealsByDateAsync for a single day, or expand the service.
            // A common pattern is to have a GetMealHistoryAsync(int userId, DateTime? start, DateTime? end)
            // For the purpose of this example, let's just return all for simplicity or require date.
            // If calling GetMealsByDateAsync, we'd need to loop or change service signature.
            return Ok(new List<MealEntryDto>()); // Placeholder
        }


        [HttpGet("{id:int}", Name = "GetMealById")] // Named route for CreatedAtAction
        public async Task<IActionResult> GetMealById(int id)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (userId == null || !int.TryParse(userId, out var parsedUserId))
            {
                return Unauthorized("Invalid user token.");
            }

            // Note: I didn't create a GetMealByIdAsync in the service yet, but you'd need one.
            // For now, assume it exists or reuse a query.
            var meal = (await _mealService.GetMealsByDateAsync(parsedUserId, DateTime.Today)) // Placeholder: better to have a dedicated GetById
                       .FirstOrDefault(m => m.Id == id);

            if (meal == null)
            {
                return NotFound("Meal not found.");
            }
            return Ok(meal);
        }


        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateMeal(int id, [FromBody] UpdateMealRequestDto request)
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

            var updatedMeal = await _mealService.UpdateMealAsync(parsedUserId, id, request);
            if (updatedMeal == null)
            {
                return NotFound("Meal not found or not authorized.");
            }

            return Ok(updatedMeal);
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteMeal(int id)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (userId == null || !int.TryParse(userId, out var parsedUserId))
            {
                return Unauthorized("Invalid user token.");
            }

            var result = await _mealService.DeleteMealAsync(parsedUserId, id);
            if (!result)
            {
                return NotFound("Meal not found or not authorized.");
            }

            return NoContent(); // 204 No Content for successful deletion
        }

        // Saved Meals Endpoints
        [HttpPost("saved")]
        public async Task<IActionResult> AddSavedMeal([FromBody] AddSavedMealRequestDto request)
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

            var savedMeal = await _mealService.AddSavedMealAsync(parsedUserId, request);
            return CreatedAtAction(nameof(GetSavedMealById), new { id = savedMeal?.Id }, savedMeal);
        }

        [HttpGet("saved")]
        public async Task<IActionResult> GetSavedMeals()
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (userId == null || !int.TryParse(userId, out var parsedUserId))
            {
                return Unauthorized("Invalid user token.");
            }

            var savedMeals = await _mealService.GetSavedMealsAsync(parsedUserId);
            return Ok(savedMeals);
        }

        [HttpGet("saved/{id:int}", Name = "GetSavedMealById")]
        public async Task<IActionResult> GetSavedMealById(int id)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (userId == null || !int.TryParse(userId, out var parsedUserId))
            {
                return Unauthorized("Invalid user token.");
            }

            // Note: You might need to add a GetSavedMealByIdAsync to IMealService
            var savedMeal = (await _mealService.GetSavedMealsAsync(parsedUserId)) // Placeholder: ideally call dedicated GetById
                            .FirstOrDefault(sm => sm.Id == id);

            if (savedMeal == null)
            {
                return NotFound("Saved meal not found.");
            }
            return Ok(savedMeal);
        }


        [HttpPut("saved/{id}")]
        public async Task<IActionResult> UpdateSavedMeal(int id, [FromBody] UpdateSavedMealRequestDto request)
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

            var updatedSavedMeal = await _mealService.UpdateSavedMealAsync(parsedUserId, id, request);
            if (updatedSavedMeal == null)
            {
                return NotFound("Saved meal not found or not authorized.");
            }

            return Ok(updatedSavedMeal);
        }

        [HttpDelete("saved/{id}")]
        public async Task<IActionResult> DeleteSavedMeal(int id)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (userId == null || !int.TryParse(userId, out var parsedUserId))
            {
                return Unauthorized("Invalid user token.");
            }

            var result = await _mealService.DeleteSavedMealAsync(parsedUserId, id);
            if (!result)
            {
                return NotFound("Saved meal not found or not authorized.");
            }

            return NoContent();
        }
    }
}