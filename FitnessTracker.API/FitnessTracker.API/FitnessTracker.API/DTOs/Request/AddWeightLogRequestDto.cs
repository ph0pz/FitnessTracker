using System.ComponentModel.DataAnnotations;

namespace FitnessTracker.API.DTOs.Request
{
    public class AddWeightLogRequestDto
    {
        [Required]
        public DateTime LogDate { get; set; }
        [Required, Range(1, 500)] // Assuming kg/lbs
        public decimal Weight { get; set; }
        [Range(1, 200)] // Assuming cm
        public decimal? WaistSizeCm { get; set; } // Optional
        [Range(1, 50)] // Assuming percentage
        public decimal? BodyFatPercentage { get; set; } // Optional
        public string Notes { get; set; }
    }

}
