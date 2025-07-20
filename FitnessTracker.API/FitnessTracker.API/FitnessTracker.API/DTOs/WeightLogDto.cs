namespace FitnessTracker.API.DTOs
{
    public class WeightLogDto
    {
        public int Id { get; set; }
        public DateTime LogDate { get; set; }
        public decimal Weight { get; set; }
        public decimal? WaistSizeCm { get; set; }
        public decimal? BodyFatPercentage { get; set; }
    }
}
