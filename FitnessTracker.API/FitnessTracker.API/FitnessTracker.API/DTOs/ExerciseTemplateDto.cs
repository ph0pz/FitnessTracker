namespace FitnessTracker.API.DTOs
{
    public class ExerciseTemplateDto
    {
        public int Id { get; set; }
        public string TemplateName { get; set; }
        public string Description { get; set; }
        public List<TemplateExerciseDetailDto> Exercises { get; set; } // For full template details
    }
}
