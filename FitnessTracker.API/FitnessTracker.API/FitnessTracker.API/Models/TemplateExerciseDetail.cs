using System;
using System.Collections.Generic;

namespace FitnessTracker.API.Models;

public partial class TemplateExerciseDetail
{
    public int Id { get; set; }

    public int ExerciseTemplateId { get; set; }

    public string ExerciseName { get; set; } = null!;

    public int? DefaultSets { get; set; }

    public int? DefaultReps { get; set; }

    public decimal? DefaultWeight { get; set; }

    public string? Notes { get; set; }

    public virtual ExerciseTemplate ExerciseTemplate { get; set; } = null!;
}
