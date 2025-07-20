using System;
using System.Collections.Generic;

namespace FitnessTracker.API.Models;

public partial class MacroGoal
{
    public int Id { get; set; }

    public int UserId { get; set; }

    public string GoalType { get; set; } = null!;

    public int Calories { get; set; }

    public int Protein { get; set; }

    public int Carbs { get; set; }

    public int Fat { get; set; }

    public DateTime SetDate { get; set; }

    public bool IsActive { get; set; }

    public virtual User User { get; set; } = null!;
}
