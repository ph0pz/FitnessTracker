using FitnessTracker.API.DTOs;
using FitnessTracker.API.DTOs.Request;
using FitnessTracker.API.Interfaces;
using FitnessTracker.API.Models;
using Microsoft.EntityFrameworkCore;

namespace FitnessTracker.API.Services
{
    public class MealService : IMealService
    {
        private readonly FitnessTrackerDbContext _context;

        public MealService(FitnessTrackerDbContext context)
        {
            _context = context;
        }

        public async Task<MealEntryDto?> AddMealAsync(int userId, AddMealRequestDto request)
        {
            var mealEntry = new MealEntry
            {
                UserId = userId,
                MealName = request.MealName,
                EntryDate = request.Date.ToUniversalTime(), // Store dates as UTC
                Calories = request.Calories,
                Protein = request.Protein,
                Carbs = request.Carbs,
                Fat = request.Fat,
                Notes = request.Notes
            };

            _context.MealEntries.Add(mealEntry);
            await _context.SaveChangesAsync();

            return new MealEntryDto
            {
                Id = mealEntry.Id,
                Date = mealEntry.EntryDate,
                MealName = mealEntry.MealName,
                Calories = mealEntry.Calories,
                Protein = mealEntry.Protein,
                Carbs = mealEntry.Carbs,
                Fat = mealEntry.Fat,
                Notes = mealEntry.Notes
            };
        }

        public async Task<List<MealEntryDto>> GetMealsByDateAsync(int userId, DateTime date)
        {
            var meals = await _context.MealEntries
                .Where(m => m.UserId == userId && m.EntryDate.Date == date.Date) // Compare date part only
                .OrderBy(m => m.EntryDate)
                .Select(m => new MealEntryDto
                {
                    Id = m.Id,
                    Date = m.EntryDate,
                    MealName = m.MealName,
                    Calories = m.Calories,
                    Protein = m.Protein,
                    Carbs = m.Carbs,
                    Fat = m.Fat,
                    Notes = m.Notes
                })
                .ToListAsync();

            return meals;
        }

        public async Task<MealEntryDto?> UpdateMealAsync(int userId, int mealId, UpdateMealRequestDto request)
        {
            var mealEntry = await _context.MealEntries.SingleOrDefaultAsync(m => m.Id == mealId && m.UserId == userId);

            if (mealEntry == null)
            {
                return null; // Meal not found or not owned by user
            }

            mealEntry.MealName = request.MealName;
            mealEntry.EntryDate = request.Date.ToUniversalTime();
            mealEntry.Calories = request.Calories;
            mealEntry.Protein = request.Protein;
            mealEntry.Carbs = request.Carbs;
            mealEntry.Fat = request.Fat;
            mealEntry.Notes = request.Notes;

            await _context.SaveChangesAsync();

            return new MealEntryDto
            {
                Id = mealEntry.Id,
                Date = mealEntry.EntryDate,
                MealName = mealEntry.MealName,
                Calories = mealEntry.Calories,
                Protein = mealEntry.Protein,
                Carbs = mealEntry.Carbs,
                Fat = mealEntry.Fat,
                Notes = mealEntry.Notes
            };
        }

        public async Task<bool> DeleteMealAsync(int userId, int mealId)
        {
            var mealEntry = await _context.MealEntries.SingleOrDefaultAsync(m => m.Id == mealId && m.UserId == userId);

            if (mealEntry == null)
            {
                return false; // Meal not found or not owned by user
            }

            _context.MealEntries.Remove(mealEntry);
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<SavedMealDto?> AddSavedMealAsync(int userId, AddSavedMealRequestDto request)
        {
            var savedMeal = new SavedMeal
            {
                UserId = userId,
                Name = request.Name,
                Calories = request.Calories,
                Protein = request.Protein,
                Carbs = request.Carbs,
                Fat = request.Fat
            };

            _context.SavedMeals.Add(savedMeal);
            await _context.SaveChangesAsync();

            return new SavedMealDto
            {
                Id = savedMeal.Id,
                Name = savedMeal.Name,
                Calories = savedMeal.Calories,
                Protein = savedMeal.Protein,
                Carbs = savedMeal.Carbs,
                Fat = savedMeal.Fat
            };
        }

        public async Task<List<SavedMealDto>> GetSavedMealsAsync(int userId)
        {
            var savedMeals = await _context.SavedMeals
                .Where(sm => sm.UserId == userId)
                .Select(sm => new SavedMealDto
                {
                    Id = sm.Id,
                    Name = sm.Name,
                    Calories = sm.Calories,
                    Protein = sm.Protein,
                    Carbs = sm.Carbs,
                    Fat = sm.Fat
                })
                .ToListAsync();

            return savedMeals;
        }

        public async Task<SavedMealDto?> UpdateSavedMealAsync(int userId, int savedMealId, UpdateSavedMealRequestDto request)
        {
            var savedMeal = await _context.SavedMeals.SingleOrDefaultAsync(sm => sm.Id == savedMealId && sm.UserId == userId);

            if (savedMeal == null)
            {
                return null; // Saved meal not found or not owned by user
            }

            savedMeal.Name = request.Name;
            savedMeal.Calories = request.Calories;
            savedMeal.Protein = request.Protein;
            savedMeal.Carbs = request.Carbs;
            savedMeal.Fat = request.Fat;

            await _context.SaveChangesAsync();

            return new SavedMealDto
            {
                Id = savedMeal.Id,
                Name = savedMeal.Name,
                Calories = savedMeal.Calories,
                Protein = savedMeal.Protein,
                Carbs = savedMeal.Carbs,
                Fat = savedMeal.Fat
            };
        }

        public async Task<bool> DeleteSavedMealAsync(int userId, int savedMealId)
        {
            var savedMeal = await _context.SavedMeals.SingleOrDefaultAsync(sm => sm.Id == savedMealId && sm.UserId == userId);

            if (savedMeal == null)
            {
                return false; // Saved meal not found or not owned by user
            }

            _context.SavedMeals.Remove(savedMeal);
            await _context.SaveChangesAsync();
            return true;
        }
    }
}



