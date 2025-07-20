using FitnessTracker.API.DTOs.Request;

namespace FitnessTracker.API.DTOs
{
    public class ExerciseEntryDto : AddExerciseRequestDto
    {
        public int Id { get; set; }
        public bool IsCompleted { get; set; } // If we add 'mark as done' later
    }
}
