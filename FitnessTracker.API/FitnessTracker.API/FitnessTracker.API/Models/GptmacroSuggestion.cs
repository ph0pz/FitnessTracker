using System;
using System.Collections.Generic;

namespace FitnessTracker.API.Models;

public partial class GptmacroSuggestion
{
    public int SuggestionId { get; set; }

    public int? UserId { get; set; }

    public string InputText { get; set; } = null!;

    public int? OutputCalories { get; set; }

    public double? OutputProtein { get; set; }

    public double? OutputCarbs { get; set; }

    public double? OutputFat { get; set; }

    public DateTime? CreatedAt { get; set; }

    public virtual User? User { get; set; }
}
