using System;
using System.Collections.Generic;

namespace FitnessTracker.API.Models;

public partial class WeightLog
{
    public int Id { get; set; }

    public int UserId { get; set; }

    public DateTime LogDate { get; set; }

    public decimal Weight { get; set; }

    public decimal? WaistSizeCm { get; set; }

    public decimal? BodyFatPercentage { get; set; }

    public string? Notes { get; set; }

    public virtual User User { get; set; } = null!;
}
