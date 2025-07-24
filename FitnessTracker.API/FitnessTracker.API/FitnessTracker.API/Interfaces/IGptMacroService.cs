using FitnessTracker.API.DTOs;

namespace FitnessTracker.API.Interfaces
{
    public interface IGptMacroService
    {
        Task<GptMacroResponseDto?> AnalyzeMacrosAsync(string prompt ,int userId);
    }
}
