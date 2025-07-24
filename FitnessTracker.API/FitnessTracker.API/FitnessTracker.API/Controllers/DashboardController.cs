using FitnessTracker.API.Interfaces;
using FitnessTracker.API.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace FitnessTracker.API.Controllers
{
    namespace FitnessTracker.Api.Controllers
    {
        [ApiController]
        [Route("api/[controller]")]
        [Authorize]
        public class DashboardController : ControllerBase
        {
            private readonly IDashboardService _dashboardService;
            private readonly IProgressService _progressService; // Inject ProgressService for weight history

            public DashboardController(IDashboardService dashboardService, IProgressService progressService)
            {
                _dashboardService = dashboardService;
                _progressService = progressService;
            }

            [HttpGet("summary")]
            public async Task<IActionResult> GetDashboardSummary([FromQuery] DateTime? date = null)
            {
                var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
                if (userId == null || !int.TryParse(userId, out var parsedUserId))
                {
                    return Unauthorized("Invalid user token.");
                }

                var summaryDate = date ?? DateTime.Today; // Default to today if no date is provided
                var summary = await _dashboardService.GetDashboardSummaryAsync(parsedUserId, summaryDate);

                if (summary == null)
                {
                    return NotFound("Dashboard summary not available for this user or date.");
                }

                return Ok(summary);
            }

            [HttpGet("weight-progress")]
            public async Task<IActionResult> GetWeightProgress([FromQuery] int lastDays = 0)
            {
                var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
                if (userId == null || !int.TryParse(userId, out var parsedUserId))
                {
                    return Unauthorized("Invalid user token.");
                }

                // The service would ideally handle the 'lastDays' filtering internally,
                // or the UI requests all history and filters client-side.
                // For now, let's assume service returns full history and client filters for simplicity.
                // If `lastDays` is > 0, the service would need to support it.
                var weightHistory = await _progressService.GetWeightHistoryAsync(parsedUserId);

                if (lastDays > 0)
                {
                    weightHistory = weightHistory.Where(w => w.LogDate >= DateTime.UtcNow.AddDays(-lastDays)).ToList();
                }

                return Ok(weightHistory);
            }
        }
    }

}
