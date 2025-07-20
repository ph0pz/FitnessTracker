using FitnessTracker.API.DTOs;
using FitnessTracker.API.DTOs.Request;

namespace FitnessTracker.API.Interfaces
{
    public interface IProgressService
    {
        Task<WeightLogDto?> AddWeightLogAsync(int userId, AddWeightLogRequestDto request);
        Task<List<WeightLogDto>> GetWeightHistoryAsync(int userId);
        Task<WeightLogDto?> UpdateWeightLogAsync(int userId, int logId, UpdateWeightLogRequestDto request);
        Task<bool> DeleteWeightLogAsync(int userId, int logId);
    }
}
