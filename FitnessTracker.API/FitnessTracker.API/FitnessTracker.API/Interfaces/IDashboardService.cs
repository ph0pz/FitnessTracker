using FitnessTracker.API.DTOs;

namespace FitnessTracker.API.Interfaces
{
    public interface IDashboardService
    {
        Task<List<DashboardSummaryResponseDto?>> GetDashboardSummaryAsync(int userId, DateTime date);
    }
}
