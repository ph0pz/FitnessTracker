using System.ComponentModel.DataAnnotations;

namespace FitnessTracker.API.DTOs.Request
{
    public class SetMacroGoalRequestDto
    {
        //[Required, StringLength(20)]
        public string GoalType { get; set; }
        [Range(1000, 10000)]
        public int Calories { get; set; }
        [Range(0, 1000)]
        public int Protein { get; set; }
        [Range(0, 1000)]
        public int Carbs { get; set; }
        [Range(0, 1000)]
        public int Fat { get; set; }
    }
}
