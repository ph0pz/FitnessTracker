using System.ComponentModel.DataAnnotations;

namespace FitnessTracker.API.DTOs.Request
{
    public class UpdateUserProfileRequestDto
    {
        [EmailAddress]
        public string Email { get; set; }
       
    }
}
