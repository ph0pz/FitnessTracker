using FitnessTracker.API.DTOs;
using FitnessTracker.API.Interfaces;
using FitnessTracker.API.Models;
using Microsoft.EntityFrameworkCore;

namespace FitnessTracker.Api.Services
{
    public class DashboardService : IDashboardService
    {
        private readonly FitnessTrackerDbContext _context;

        public DashboardService(FitnessTrackerDbContext context)
        {
            _context = context;
        }

        public async Task<DashboardSummaryResponseDto?> GetDashboardSummaryAsync(int userId, DateTime date)
        {
            var user = await _context.Users.Include(u => u.UserProfile)
                                    .Include(u => u.MacroGoals.Where(mg => mg.IsActive))
                                    .AsNoTracking()
                                    .FirstOrDefaultAsync(u => u.Id == userId);

            if (user == null)
            {
                return null;
            }

            var todayUtc = date.Date; // Use date part for daily summary

            // Consumed Macros
            var mealsToday = await _context.MealEntries
                .Where(m => m.UserId == userId && m.EntryDate.Date == todayUtc)
                .ToListAsync();

            var caloriesConsumed = mealsToday.Sum(m => m.Calories);
            var proteinConsumed = mealsToday.Sum(m => m.Protein);
            var carbsConsumed = mealsToday.Sum(m => m.Carbs);
            var fatConsumed = mealsToday.Sum(m => m.Fat);

            // Goal Macros
            var activeMacroGoal = user.MacroGoals.FirstOrDefault();
            var dailyCalorieGoal = activeMacroGoal?.Calories ?? user.UserProfile?.DailyCalorieTarget ?? 0;
            var dailyProteinGoal = activeMacroGoal?.Protein ?? user.UserProfile?.DailyProteinTarget ?? 0;
            var dailyCarbGoal = activeMacroGoal?.Carbs ?? user.UserProfile?.DailyCarbTarget ?? 0;
            var dailyFatGoal = activeMacroGoal?.Fat ?? user.UserProfile?.DailyFatTarget ?? 0;

            // Exercises
            var completedExercisesToday = await _context.ExerciseEntries
                .Where(e => e.UserId == userId && e.EntryDate.Date == todayUtc && e.IsCompleted)
                .CountAsync();

            // Current Weight
            var latestWeightLog = await _context.WeightLogs
                .Where(wl => wl.UserId == userId)
                .OrderByDescending(wl => wl.LogDate)
                .Select(wl => (decimal?)wl.Weight) // Cast to nullable decimal
                .FirstOrDefaultAsync();

            return new DashboardSummaryResponseDto
            {
                Username = user.Username,
                TodayDate = todayUtc,
                CaloriesConsumed = caloriesConsumed,
                ProteinConsumed = proteinConsumed,
                CarbsConsumed = carbsConsumed,
                FatConsumed = fatConsumed,
                DailyCalorieGoal = dailyCalorieGoal,
                DailyProteinGoal = dailyProteinGoal,
                DailyCarbGoal = dailyCarbGoal,
                DailyFatGoal = dailyFatGoal,
                CompletedExercisesToday = completedExercisesToday,
                CurrentWeight = latestWeightLog
            };
        }
    }
}
