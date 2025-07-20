using System;
using System.Collections.Generic;

namespace FitnessTracker.API.Models;

public partial class ExerciseEntry
{
    public int Id { get; set; }

    public int UserId { get; set; }

    public DateTime EntryDate { get; set; }

    public string ExerciseName { get; set; } = null!;

    public int Sets { get; set; }

    public int Reps { get; set; }

    public decimal? Weight { get; set; }

    public int? DurationMinutes { get; set; }

    public bool IsCompleted { get; set; }

    public string? Notes { get; set; }

    public virtual User User { get; set; } = null!;
}
