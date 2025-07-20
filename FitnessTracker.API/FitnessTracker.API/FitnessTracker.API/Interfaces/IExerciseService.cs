using FitnessTracker.API.DTOs;
using FitnessTracker.API.DTOs.Request;

namespace FitnessTracker.API.Interfaces
{
    public interface IExerciseService
    {
        Task<ExerciseEntryDto?> AddExerciseAsync(int userId, AddExerciseRequestDto request);
        Task<List<ExerciseEntryDto>> GetExercisesByDateAsync(int userId, DateTime date);
        Task<ExerciseEntryDto?> UpdateExerciseAsync(int userId, int exerciseId, AddExerciseRequestDto request);
        Task<bool> DeleteExerciseAsync(int userId, int exerciseId);
        Task<bool> MarkExerciseAsCompletedAsync(int userId, int exerciseId, bool isCompleted);

        Task<ExerciseTemplateDto?> CreateExerciseTemplateAsync(int userId, CreateExerciseTemplateRequestDto request);
        Task<List<ExerciseTemplateDto>> GetExerciseTemplatesAsync(int userId);
        Task<ExerciseTemplateDto?> GetExerciseTemplateByIdAsync(int userId, int templateId);
        Task<ExerciseTemplateDto?> UpdateExerciseTemplateAsync(int userId, int templateId, UpdateExerciseTemplateRequestDto request);
        Task<bool> DeleteExerciseTemplateAsync(int userId, int templateId);
    }
}
