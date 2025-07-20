using FitnessTracker.API.DTOs;

namespace FitnessTracker.API.Interfaces
{
    public interface IDashboardService
    {
        Task<DashboardSummaryResponseDto?> GetDashboardSummaryAsync(int userId, DateTime date);
    }
}
