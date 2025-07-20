using System;
using System.Collections.Generic;

namespace FitnessTracker.API.Models;

public partial class ExerciseTemplate
{
    public int Id { get; set; }

    public int UserId { get; set; }

    public string TemplateName { get; set; } = null!;

    public string? Description { get; set; }

    public DateTime CreatedAt { get; set; }

    public virtual ICollection<TemplateExerciseDetail> TemplateExerciseDetails { get; set; } = new List<TemplateExerciseDetail>();

    public virtual User User { get; set; } = null!;
}
