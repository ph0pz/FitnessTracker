using System.ComponentModel.DataAnnotations;

namespace FitnessTracker.API.DTOs.Request
{
    public class CreateExerciseTemplateRequestDto
    {
        [Required, StringLength(150)]
        public string TemplateName { get; set; }
        public string Description { get; set; }
        public List<CreateTemplateExerciseDetailDto> Exercises { get; set; } // List of exercises in the template
    }
}
