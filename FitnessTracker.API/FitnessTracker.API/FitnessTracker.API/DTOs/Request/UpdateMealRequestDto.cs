using System.ComponentModel.DataAnnotations;

namespace FitnessTracker.API.DTOs.Request
{
    public class UpdateMealRequestDto
    {
        [Required]
        public int Id { get; set; } // ID of the meal to update
                                    // All other fields from AddMealRequestDto can be here, or use property-level validation
        [Required, StringLength(100)]
        public string MealName { get; set; }
        [Range(0, 10000)]
        public int Calories { get; set; }
        [Range(0, 1000)]
        public int Protein { get; set; }
        [Range(0, 1000)]
        public int Carbs { get; set; }
        [Range(0, 1000)]
        public int Fat { get; set; }
        public string Notes { get; set; }
        public DateTime Date { get; set; } // Date can also be updated
    }
}
