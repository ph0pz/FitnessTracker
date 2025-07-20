namespace FitnessTracker.API.Services
{

    using global::FitnessTracker.API.DTOs;
    using global::FitnessTracker.API.DTOs.Request;
    using global::FitnessTracker.API.Interfaces;
    using global::FitnessTracker.API.Models;
    using Microsoft.EntityFrameworkCore;

    namespace FitnessTracker.Api.Services
    {
        public class MacroGoalService : IMacroGoalService
        {
            private readonly FitnessTrackerDbContext _context;

            public MacroGoalService(FitnessTrackerDbContext context)
            {
                _context = context;
            }

            public async Task<MacroGoalDto?> GetCurrentMacroGoalAsync(int userId)
            {
                var currentGoal = await _context.MacroGoals
                    .Where(mg => mg.UserId == userId && mg.IsActive)
                    .OrderByDescending(mg => mg.SetDate) // Just in case multiple are active, take the latest
                    .Select(mg => new MacroGoalDto
                    {
                        Id = mg.Id,
                        GoalType = mg.GoalType,
                        Calories = mg.Calories,
                        Protein = mg.Protein,
                        Carbs = mg.Carbs,
                        Fat = mg.Fat,
                        SetDate = mg.SetDate,
                        IsActive = mg.IsActive
                    })
                    .FirstOrDefaultAsync();

                // If no active goal, fall back to UserProfile defaults
                if (currentGoal == null)
                {
                    var userProfile = await _context.UserProfiles
                        .AsNoTracking()
                        .FirstOrDefaultAsync(up => up.UserId == userId);

                    if (userProfile != null)
                    {
                        return new MacroGoalDto
                        {
                            Id = 0, // Indicate it's a default, not from MacroGoals table
                            GoalType = "Maintain", // Default type
                            Calories = userProfile.DailyCalorieTarget,
                            Protein = userProfile.DailyProteinTarget,
                            Carbs = userProfile.DailyCarbTarget,
                            Fat = userProfile.DailyFatTarget,
                            SetDate = DateTime.MinValue, // Indicate no specific set date
                            IsActive = true
                        };
                    }
                }

                return currentGoal;
            }

            public async Task<MacroGoalDto?> SetMacroGoalAsync(int userId, SetMacroGoalRequestDto request)
            {
                // Deactivate all previous active goals for this user
                var activeGoals = await _context.MacroGoals
                    .Where(mg => mg.UserId == userId && mg.IsActive)
                    .ToListAsync();

                foreach (var goal in activeGoals)
                {
                    goal.IsActive = false;
                }

                // Create new active goal
                var newGoal = new MacroGoal
                {
                    UserId = userId,
                    GoalType = request.GoalType,
                    Calories = request.Calories,
                    Protein = request.Protein,
                    Carbs = request.Carbs,
                    Fat = request.Fat,
                    SetDate = DateTime.UtcNow,
                    IsActive = true
                };

                _context.MacroGoals.Add(newGoal);
                await _context.SaveChangesAsync();

                return new MacroGoalDto
                {
                    Id = newGoal.Id,
                    GoalType = newGoal.GoalType,
                    Calories = newGoal.Calories,
                    Protein = newGoal.Protein,
                    Carbs = newGoal.Carbs,
                    Fat = newGoal.Fat,
                    SetDate = newGoal.SetDate,
                    IsActive = newGoal.IsActive
                };
            }

            public async Task<List<MacroGoalDto>> GetMacroGoalHistoryAsync(int userId)
            {
                var history = await _context.MacroGoals
                    .Where(mg => mg.UserId == userId)
                    .OrderByDescending(mg => mg.SetDate)
                    .Select(mg => new MacroGoalDto
                    {
                        Id = mg.Id,
                        GoalType = mg.GoalType,
                        Calories = mg.Calories,
                        Protein = mg.Protein,
                        Carbs = mg.Carbs,
                        Fat = mg.Fat,
                        SetDate = mg.SetDate,
                        IsActive = mg.IsActive
                    })
                    .ToListAsync();

                return history;
            }
        }
    }
}