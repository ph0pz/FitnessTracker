using FitnessTracker.API.DTOs.Request;
using System.ComponentModel.DataAnnotations;

namespace FitnessTracker.API.DTOs
{
    public class UpdateWeightLogRequestDto : AddWeightLogRequestDto
    {
        [Required]
        public int Id { get; set; } // ID of the log to update
    }
}
