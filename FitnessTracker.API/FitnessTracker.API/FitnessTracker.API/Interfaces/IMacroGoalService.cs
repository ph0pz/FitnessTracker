using FitnessTracker.API.DTOs;
using FitnessTracker.API.DTOs.Request;

namespace FitnessTracker.API.Interfaces
{
    public interface IMacroGoalService
    {
        Task<MacroGoalDto?> GetCurrentMacroGoalAsync(int userId);
        Task<MacroGoalDto?> SetMacroGoalAsync(int userId, SetMacroGoalRequestDto request);
        Task<List<MacroGoalDto>> GetMacroGoalHistoryAsync(int userId);
    }
}
