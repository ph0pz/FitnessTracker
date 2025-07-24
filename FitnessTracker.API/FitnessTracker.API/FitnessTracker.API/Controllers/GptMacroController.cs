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
    public class GptMacroController : ControllerBase
    {
        private readonly IGptMacroService _gptMacroService;
    

        public GptMacroController(IGptMacroService gptMacroService)
        {
            _gptMacroService = gptMacroService;
        
        }

        [HttpPost("analyze-macros")]
        public async Task<IActionResult> AnalyzeMacros([FromBody] GptRequest request)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (userId == null || !int.TryParse(userId, out var parsedUserId))
            {
                return Unauthorized("Invalid user token.");
            }
            var macroData = await _gptMacroService.AnalyzeMacrosAsync(request.Prompt , parsedUserId);
            if (macroData == null)
                return BadRequest("GPT analysis failed");

            return Ok(macroData);
        }
    }
}