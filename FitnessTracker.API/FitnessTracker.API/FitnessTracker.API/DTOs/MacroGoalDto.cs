namespace FitnessTracker.API.DTOs
{
    public class MacroGoalDto
    {
        public int Id { get; set; }
        public string GoalType { get; set; } // e.g., "Maintain", "Cut", "Bulk"
        public int Calories { get; set; }
        public int Protein { get; set; }
        public int Carbs { get; set; }
        public int Fat { get; set; }
        public DateTime SetDate { get; set; }
        public bool IsActive { get; set; }
    }
}
