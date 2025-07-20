using System.ComponentModel.DataAnnotations;

namespace FitnessTracker.API.DTOs
{
    public class CreateTemplateExerciseDetailDto
    {
        [Required, StringLength(100)]
        public string ExerciseName { get; set; }
        [Range(1, 99)]
        public int? DefaultSets { get; set; }
        [Range(1, 999)]
        public int? DefaultReps { get; set; }
        [Range(0, 999.99)]
        public decimal? DefaultWeight { get; set; }
    }
}
