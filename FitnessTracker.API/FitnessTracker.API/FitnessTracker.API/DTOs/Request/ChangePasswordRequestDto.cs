using System.ComponentModel.DataAnnotations;

namespace FitnessTracker.API.DTOs.Request
{
    public class ChangePasswordRequestDto
    {
        [Required]
        public string CurrentPassword { get; set; }
        [Required, StringLength(100, MinimumLength = 6)] // Enforce strong password policy
        public string NewPassword { get; set; }
        [Required, Compare("NewPassword")]
        public string ConfirmNewPassword { get; set; }
    }
}
