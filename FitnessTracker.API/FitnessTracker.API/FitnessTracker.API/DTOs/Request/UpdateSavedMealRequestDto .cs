using System.ComponentModel.DataAnnotations;

namespace FitnessTracker.API.DTOs.Request
{
    public class UpdateSavedMealRequestDto : AddSavedMealRequestDto
    {
        [Required]
        public int Id { get; set; } // ID of the saved meal to update
    }
}
