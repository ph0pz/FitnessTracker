using FitnessTracker.API.DTOs.Request;

namespace FitnessTracker.API.DTOs
{
    public class MealEntryDto : AddMealRequestDto
    {
        public int Id { get; set; }
        // Inherits Date, MealName, Calories, Protein, Carbs, Fat, Notes
    }
}
