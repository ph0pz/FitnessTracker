using FitnessTracker.API.DTOs;
using FitnessTracker.API.DTOs.Request;

namespace FitnessTracker.API.Interfaces
{
    public interface IMealService
    {
        Task<MealEntryDto?> AddMealAsync(int userId, AddMealRequestDto request);
        Task<List<MealEntryDto>> GetMealsByDateAsync(int userId, DateTime date);
        Task<MealEntryDto?> UpdateMealAsync(int userId, int mealId, UpdateMealRequestDto request);
        Task<bool> DeleteMealAsync(int userId, int mealId);
        Task<SavedMealDto?> AddSavedMealAsync(int userId, AddSavedMealRequestDto request);
        Task<List<SavedMealDto>> GetSavedMealsAsync(int userId);
        Task<SavedMealDto?> UpdateSavedMealAsync(int userId, int savedMealId, UpdateSavedMealRequestDto request);
        Task<bool> DeleteSavedMealAsync(int userId, int savedMealId);
    }
}
