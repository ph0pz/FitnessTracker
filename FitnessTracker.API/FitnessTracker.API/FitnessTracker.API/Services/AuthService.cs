using FitnessTracker.API.DTOs;
using FitnessTracker.API.DTOs.Request;
using FitnessTracker.API.DTOs.Request.FitnessTracker.Api.DTOs.Auth;
using FitnessTracker.API.Helpers;
using FitnessTracker.API.Interfaces;
using FitnessTracker.API.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using System.IdentityModel.Tokens.Jwt;

namespace FitnessTracker.API.Services
{
    public class AuthService : IAuthService
    {
        private readonly FitnessTrackerDbContext _context;
        private readonly JwtHelper _jwtHelper;

        public AuthService(FitnessTrackerDbContext context, JwtHelper jwtHelper)
        {
            _context = context;
            _jwtHelper = jwtHelper;
        }

        public async Task<TokenResponseDto?> LoginAsync(LoginRequestDto request)
        {
            var user = await _context.Users.SingleOrDefaultAsync(u => u.Username == request.Username);

            if (user == null || !PasswordHasher.VerifyPassword(request.Password, user.PasswordHash))
            {
                return null; // Invalid credentials
            }

            var token = _jwtHelper.GenerateJwtToken(user.Id, user.Username);
            return new TokenResponseDto { Token = token, UserId = user.Id };
        }

        public async Task<bool> RegisterAsync(RegisterRequestDto request)
        {
            if (await _context.Users.AnyAsync(u => u.Username == request.Username || u.Email == request.Email))
            {
                return false; // Username or Email already exists
            }

            var passwordHash = PasswordHasher.HashPassword(request.Password);

            var user = new User
            {
                Username = request.Username,
                Email = request.Email,
                PasswordHash = passwordHash,
                CreatedAt = DateTime.UtcNow // Store as UTC
            };

            _context.Users.Add(user);
            await _context.SaveChangesAsync();

            // Create a default user profile after registration
            var userProfile = new UserProfile
            {
                UserId = user.Id,
                DailyCalorieTarget = 2000,
                DailyProteinTarget = 150,
                DailyCarbTarget = 200,
                DailyFatTarget = 60
            };
            _context.UserProfiles.Add(userProfile);
            await _context.SaveChangesAsync();

            // Also set a default active macro goal
            var defaultMacroGoal = new MacroGoal
            {
                UserId = user.Id,
                GoalType = "Maintain", // Default
                Calories = 2000,
                Protein = 150,
                Carbs = 200,
                Fat = 60,
                SetDate = DateTime.UtcNow,
                IsActive = true
            };
            _context.MacroGoals.Add(defaultMacroGoal);
            await _context.SaveChangesAsync();

            return true;
        }

        public async Task<UserProfileResponseDto?> GetUserProfileAsync(int userId)
        {
            var user = await _context.Users
                .Include(u => u.UserProfile)
                .Include(u => u.MacroGoals.Where(mg => mg.IsActive)) // Get active goal for display
                .AsNoTracking()
                .SingleOrDefaultAsync(u => u.Id == userId);

            if (user == null || user.UserProfile == null)
            {
                return null;
            }

            var activeGoal = user.MacroGoals.FirstOrDefault();

            return new UserProfileResponseDto
            {
                Username = user.Username,
                Email = user.Email,
                DailyCalorieTarget = activeGoal?.Calories ?? user.UserProfile.DailyCalorieTarget,
                DailyProteinTarget = activeGoal?.Protein ?? user.UserProfile.DailyProteinTarget,
                DailyCarbTarget = activeGoal?.Carbs ?? user.UserProfile.DailyCarbTarget,
                DailyFatTarget = activeGoal?.Fat ?? user.UserProfile.DailyFatTarget
            };
        }

        public async Task<UserProfileResponseDto?> UpdateUserProfileAsync(int userId, UpdateUserProfileRequestDto request)
        {
            var user = await _context.Users.SingleOrDefaultAsync(u => u.Id == userId);

            if (user == null)
            {
                return null;
            }

            // Only update email here, other profile fields should be in UserProfile or MacroGoal service
            if (!string.IsNullOrWhiteSpace(request.Email) && user.Email != request.Email)
            {
                if (await _context.Users.AnyAsync(u => u.Email == request.Email && u.Id != userId))
                {
                    // You might throw a custom exception here to return a specific error
                    return null; // Email already taken by another user
                }
                user.Email = request.Email;
            }

            await _context.SaveChangesAsync();
            return await GetUserProfileAsync(userId); // Return updated profile
        }

        public async Task<bool> ChangePasswordAsync(int userId, ChangePasswordRequestDto request)
        {
            var user = await _context.Users.SingleOrDefaultAsync(u => u.Id == userId);

            if (user == null || !PasswordHasher.VerifyPassword(request.CurrentPassword, user.PasswordHash))
            {
                return false; // User not found or current password incorrect
            }

            user.PasswordHash = PasswordHasher.HashPassword(request.NewPassword);
            await _context.SaveChangesAsync();
            return true;
        }
    }

}
