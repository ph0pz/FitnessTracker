namespace FitnessTracker.API.DTOs
{
    public class TemplateExerciseDetailDto
    {
        public int Id { get; set; }
        public string ExerciseName { get; set; }
        public int? DefaultSets { get; set; }
        public int? DefaultReps { get; set; }
        public decimal? DefaultWeight { get; set; }
    }
}
