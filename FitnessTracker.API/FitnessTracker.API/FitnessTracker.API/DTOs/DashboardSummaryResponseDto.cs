namespace FitnessTracker.API.DTOs
{

    public class DashboardSummaryResponseDto
    {
        public string Username { get; set; }
        public DateTime TodayDate { get; set; }
        public int CaloriesConsumed { get; set; }
        public int ProteinConsumed { get; set; }
        public int CarbsConsumed { get; set; }
        public int FatConsumed { get; set; }
        public int DailyCalorieGoal { get; set; }
        public int DailyProteinGoal { get; set; }
        public int DailyCarbGoal { get; set; }
        public int DailyFatGoal { get; set; }
        public int CompletedExercisesToday { get; set; } 
        public decimal? CurrentWeight { get; set; } 
    }

 

}
