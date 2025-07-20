using System.ComponentModel.DataAnnotations;

namespace FitnessTracker.API.DTOs.Request
{
    namespace FitnessTracker.Api.DTOs.Auth
    {
        public class LoginRequestDto
        {
            [Required(ErrorMessage = "Username is required.")]
            [StringLength(50, MinimumLength = 3, ErrorMessage = "Username must be between 3 and 50 characters.")]
            public string Username { get; set; } = string.Empty; // Initialize to prevent null warnings

            [Required(ErrorMessage = "Password is required.")]
            [StringLength(100, MinimumLength = 6, ErrorMessage = "Password must be at least 6 characters long.")]
            // Note: Password validation here is for format/length. Actual strength checks might be done on registration.
            public string Password { get; set; } = string.Empty;
        }
    }
}
