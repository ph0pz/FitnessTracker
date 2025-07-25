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

        public async Task<List<DashboardSummaryResponseDto>> GetDashboardSummaryAsync(int userId, DateTime todayDate)
        {
            var summaries = new List<DashboardSummaryResponseDto>();

            // Fetch user and active macro goals once
            var user = await _context.Users
                                     .Include(u => u.UserProfile)
                                     .Include(u => u.MacroGoals.Where(mg => mg.IsActive))
                                     .AsNoTracking()
                                     .FirstOrDefaultAsync(u => u.Id == userId);

            if (user == null)
            {
                return summaries; // Return empty list if user not found
            }

            // Determine the 30-day range (from 29 days ago up to and including today)
            var todayUtc = todayDate.Date;
            var thirtyDaysAgoUtc = todayUtc.AddDays(-29); // Start date for 30 days (today is 1st day)

            // Fetch all relevant data for the 30-day period in single queries for efficiency
            var allMealsInPeriod = await _context.MealEntries
                .Where(m => m.UserId == userId && m.EntryDate.Date >= thirtyDaysAgoUtc && m.EntryDate.Date <= todayUtc)
                .ToListAsync();

            var allExercisesInPeriod = await _context.ExerciseEntries
                .Where(e => e.UserId == userId && e.EntryDate.Date >= thirtyDaysAgoUtc && e.EntryDate.Date <= todayUtc && e.IsCompleted)
                .ToListAsync();

            // Fetch all weight logs up to todayUtc (to find the latest on or before each day)
            var allWeightLogs = await _context.WeightLogs
                .Where(wl => wl.UserId == userId && wl.LogDate.Date <= todayUtc)
                .OrderByDescending(wl => wl.LogDate)
                .ToListAsync(); // Fetch all and filter in memory

            // Goal Macros (these are usually static for the period based on the active goal)
            var activeMacroGoal = user.MacroGoals.FirstOrDefault();
            var dailyCalorieGoal = activeMacroGoal?.Calories ?? user.UserProfile?.DailyCalorieTarget ?? 0;
            var dailyProteinGoal = activeMacroGoal?.Protein ?? user.UserProfile?.DailyProteinTarget ?? 0;
            var dailyCarbGoal = activeMacroGoal?.Carbs ?? user.UserProfile?.DailyCarbTarget ?? 0;
            var dailyFatGoal = activeMacroGoal?.Fat ?? user.UserProfile?.DailyFatTarget ?? 0;

            // Iterate through each day in the 30-day range
            for (DateTime currentDate = thirtyDaysAgoUtc; currentDate <= todayUtc; currentDate = currentDate.AddDays(1))
            {
                // Consumed Macros for the current day
                var mealsForDay = allMealsInPeriod.Where(m => m.EntryDate.Date == currentDate).ToList();
                var caloriesConsumed = mealsForDay.Sum(m => m.Calories);
                var proteinConsumed = mealsForDay.Sum(m => m.Protein);
                var carbsConsumed = mealsForDay.Sum(m => m.Carbs);
                var fatConsumed = mealsForDay.Sum(m => m.Fat);

                // Exercises for the current day
                var completedExercisesForDay = allExercisesInPeriod.Count(e => e.EntryDate.Date == currentDate);

                // Current Weight: Find the latest weight log on or before the current date
                var latestWeightForDay = allWeightLogs
                    .FirstOrDefault(wl => wl.LogDate.Date <= currentDate)?.Weight;

                // ** NEW LOGIC: Only add to summaries if there was any activity (macros or exercises) for the day **
                if (caloriesConsumed > 0 || proteinConsumed > 0 || carbsConsumed > 0 || fatConsumed > 0 || completedExercisesForDay > 0)
                {
                    summaries.Add(new DashboardSummaryResponseDto
                    {
                        Username = user.Username,
                        TodayDate = currentDate, // This is now the specific date for this entry
                        CaloriesConsumed = caloriesConsumed,
                        ProteinConsumed = proteinConsumed,
                        CarbsConsumed = carbsConsumed,
                        FatConsumed = fatConsumed,
                        DailyCalorieGoal = dailyCalorieGoal,
                        // These goals are fixed for the user's active goal, not specific to the day's consumption
                        DailyProteinGoal = dailyProteinGoal,
                        DailyCarbGoal = dailyCarbGoal,
                        DailyFatGoal = dailyFatGoal,
                        CompletedExercisesToday = completedExercisesForDay,
                        CurrentWeight = latestWeightForDay // nullable decimal
                    });
                }
                // else: If no meals were logged AND no exercises were completed for this 'currentDate',
                //       we simply skip adding a DashboardSummaryResponseDto for this day.
            }

            return summaries;
        }
    }
}
