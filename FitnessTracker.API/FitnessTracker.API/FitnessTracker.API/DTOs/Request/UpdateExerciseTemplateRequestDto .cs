using System.ComponentModel.DataAnnotations;

namespace FitnessTracker.API.DTOs.Request
{
    public class UpdateExerciseTemplateRequestDto : CreateExerciseTemplateRequestDto
    {
        [Required]
        public int Id { get; set; }
        // You might also have a list of UpdateTemplateExerciseDetailDto if sub-items have IDs
        public List<UpdateTemplateExerciseDetailDto> Exercises { get; set; }
    }
}
