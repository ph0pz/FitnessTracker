using System;
using System.Collections.Generic;

namespace FitnessTracker.API.Models;

public partial class MealEntry
{
    public int Id { get; set; }

    public int UserId { get; set; }

    public string MealName { get; set; } = null!;

    public DateTime EntryDate { get; set; }

    public int Calories { get; set; }

    public int Protein { get; set; }

    public int Carbs { get; set; }

    public int Fat { get; set; }

    public string? Notes { get; set; }

    public virtual User User { get; set; } = null!;
}
