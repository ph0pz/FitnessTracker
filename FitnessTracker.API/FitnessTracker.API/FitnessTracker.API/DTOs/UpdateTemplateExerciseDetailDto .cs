namespace FitnessTracker.API.DTOs
{
    public class UpdateTemplateExerciseDetailDto : CreateTemplateExerciseDetailDto
    {
        public int? Id { get; set; } // Null for new items, ID for existing
    }
}
