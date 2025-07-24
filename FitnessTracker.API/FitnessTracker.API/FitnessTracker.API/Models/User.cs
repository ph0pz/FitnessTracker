using System;
using System.Collections.Generic;

namespace FitnessTracker.API.Models;

public partial class User
{
    public int Id { get; set; }

    public string Username { get; set; } = null!;

    public string Email { get; set; } = null!;

    public string PasswordHash { get; set; } = null!;

    public DateTime CreatedAt { get; set; }

    public virtual ICollection<ExerciseEntry> ExerciseEntries { get; set; } = new List<ExerciseEntry>();

    public virtual ICollection<ExerciseTemplate> ExerciseTemplates { get; set; } = new List<ExerciseTemplate>();

    public virtual ICollection<GptmacroSuggestion> GptmacroSuggestions { get; set; } = new List<GptmacroSuggestion>();

    public virtual ICollection<MacroGoal> MacroGoals { get; set; } = new List<MacroGoal>();

    public virtual ICollection<MealEntry> MealEntries { get; set; } = new List<MealEntry>();

    public virtual ICollection<SavedMeal> SavedMeals { get; set; } = new List<SavedMeal>();

    public virtual UserProfile? UserProfile { get; set; }

    public virtual ICollection<WeightLog> WeightLogs { get; set; } = new List<WeightLog>();
}
