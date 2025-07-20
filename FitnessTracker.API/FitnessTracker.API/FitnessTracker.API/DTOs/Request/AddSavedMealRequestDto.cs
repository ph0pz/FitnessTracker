using System.ComponentModel.DataAnnotations;

namespace FitnessTracker.API.DTOs.Request
{
    public class AddSavedMealRequestDto
    {
        [Required, StringLength(150)]
        public string Name { get; set; }

        [Range(0, 10000)]
        public int Calories { get; set; }

        [Range(0, 1000)]
        public int Protein{ get; set; }

        [Range(0, 1000)]
        public int Carbs { get; set; }

        [Range(0, 1000)]
        public int Fat { get; set; }
    }
}
