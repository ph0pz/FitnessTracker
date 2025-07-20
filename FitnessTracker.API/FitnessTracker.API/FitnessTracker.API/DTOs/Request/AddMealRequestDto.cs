using System.ComponentModel.DataAnnotations;

namespace FitnessTracker.API.DTOs.Request
{
    public class AddMealRequestDto
    {
        [Required]
        public DateTime Date { get; set; }
        [Required, StringLength(100)]
        public string MealName { get; set; } // e.g., "Lunch"
        [Range(0, 10000)]
        public int Calories { get; set; }
        [Range(0, 1000)]
        public int Protein { get; set; }
        [Range(0, 1000)]
        public int Carbs { get; set; }
        [Range(0, 1000)]
        public int Fat { get; set; }
        public string Notes { get; set; } // Optional
    }
}
