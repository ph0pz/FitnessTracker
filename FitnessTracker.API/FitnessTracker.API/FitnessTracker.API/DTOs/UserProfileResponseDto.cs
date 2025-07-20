namespace FitnessTracker.API.DTOs
{
    public class UserProfileResponseDto
    {
        public string Username { get; set; }
        public string Email { get; set; }
        // You might include current daily macro targets here too, as shown in the wireframe,
        // or rely on a separate call to /api/macro-goals/current
        public int DailyCalorieTarget { get; set; }
        public int DailyProteinTarget { get; set; }
        public int DailyCarbTarget { get; set; }
        public int DailyFatTarget { get; set; }
    }
}
