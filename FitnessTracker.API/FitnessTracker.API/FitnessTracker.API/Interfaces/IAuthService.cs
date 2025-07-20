using FitnessTracker.API.DTOs;
using FitnessTracker.API.DTOs.Request;
using FitnessTracker.API.DTOs.Request.FitnessTracker.Api.DTOs.Auth;

namespace FitnessTracker.API.Interfaces
{
    public interface IAuthService
    {
        Task<TokenResponseDto?> LoginAsync(LoginRequestDto request);
        Task<bool> RegisterAsync(RegisterRequestDto request);
        Task<UserProfileResponseDto?> GetUserProfileAsync(int userId);
        Task<UserProfileResponseDto?> UpdateUserProfileAsync(int userId, UpdateUserProfileRequestDto request);
        Task<bool> ChangePasswordAsync(int userId, ChangePasswordRequestDto request);
    }
}
