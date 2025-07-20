namespace FitnessTracker.API.DTOs
{
    public class TokenResponseDto
    {
        public string Token { get; set; } = string.Empty;
        public int UserId { get; set; }
        // You might add other user-related info here if needed by the client immediately after login
        // public string Username { get; set; } = string.Empty;
        // public string Email { get; set; } = string.Empty;
    }
}
