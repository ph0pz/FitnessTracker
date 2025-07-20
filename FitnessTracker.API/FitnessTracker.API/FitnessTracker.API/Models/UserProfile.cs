using System;
using System.Collections.Generic;

namespace FitnessTracker.API.Models;

public partial class UserProfile
{
    public int Id { get; set; }

    public int UserId { get; set; }

    public int DailyCalorieTarget { get; set; }

    public int DailyProteinTarget { get; set; }

    public int DailyCarbTarget { get; set; }

    public int DailyFatTarget { get; set; }

    public virtual User User { get; set; } = null!;
}
