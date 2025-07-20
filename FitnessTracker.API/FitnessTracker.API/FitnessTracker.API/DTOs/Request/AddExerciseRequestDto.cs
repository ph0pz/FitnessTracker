using System.ComponentModel.DataAnnotations;

namespace FitnessTracker.API.DTOs.Request
{
    public class AddExerciseRequestDto
    {
        [Required]
        public DateTime EntryDate { get; set; }
        [Required, StringLength(100)]
        public string ExerciseName { get; set; }
        [Range(1, 99)]
        public int Sets { get; set; }
        [Range(1, 999)]
        public int Reps { get; set; }
        [Range(0, 999.99)]
        public decimal? Weight { get; set; } // Optional for bodyweight exercises
        [Range(0, 1440)] // Max 24 hours
        public int? DurationMinutes { get; set; } // Optional for cardio
        public string Notes { get; set; } // Optional
    }
}
