using FitnessTracker.API.DTOs;
using FitnessTracker.API.DTOs.Request;
using FitnessTracker.API.Interfaces;
using FitnessTracker.API.Models;
using Microsoft.EntityFrameworkCore;

namespace FitnessTracker.API.Services
{
    public class ProgressService : IProgressService
    {
        private readonly FitnessTrackerDbContext _context;

        public ProgressService(FitnessTrackerDbContext context)
        {
            _context = context;
        }

        public async Task<WeightLogDto?> AddWeightLogAsync(int userId, AddWeightLogRequestDto request)
        {
           
            var requestDateUtc = request.LogDate.ToUniversalTime().Date;

           
            var existingWeightLog = await _context.WeightLogs
                .Where(wl => wl.UserId == userId && wl.LogDate.Date == requestDateUtc) 
                .FirstOrDefaultAsync();

            WeightLog weightLog;

            if (existingWeightLog != null)
            {
     
                weightLog = existingWeightLog; 
                weightLog.Weight = request.Weight;
                weightLog.WaistSizeCm = request.WaistSizeCm;
                weightLog.BodyFatPercentage = request.BodyFatPercentage;
                weightLog.Notes = request.Notes; 

                _context.WeightLogs.Update(weightLog); 
            }
            else
            {
              
                weightLog = new WeightLog
                {
                    UserId = userId,
                    LogDate = request.LogDate.ToUniversalTime(),
                    Weight = request.Weight,
                    WaistSizeCm = request.WaistSizeCm,
                    BodyFatPercentage = request.BodyFatPercentage,
                    Notes = request.Notes
                };
                _context.WeightLogs.Add(weightLog); 
            }

            await _context.SaveChangesAsync();

        
            return new WeightLogDto
            {
                Id = weightLog.Id,
                LogDate = weightLog.LogDate,
                Weight = weightLog.Weight,
                WaistSizeCm = weightLog.WaistSizeCm,
                BodyFatPercentage = weightLog.BodyFatPercentage,
               
            };
        }

        public async Task<List<WeightLogDto>> GetWeightHistoryAsync(int userId)
        {
            var history = await _context.WeightLogs
                .Where(wl => wl.UserId == userId)
                .OrderBy(wl => wl.LogDate) // Order by date for graph
                .Select(wl => new WeightLogDto
                {
                    Id = wl.Id,
                    LogDate = wl.LogDate,
                    Weight = wl.Weight,
                    WaistSizeCm = wl.WaistSizeCm,
                    BodyFatPercentage = wl.BodyFatPercentage,
                   
                })
                .ToListAsync();

            return history;
        }

        public async Task<WeightLogDto?> UpdateWeightLogAsync(int userId, int logId, UpdateWeightLogRequestDto request)
        {
            var weightLog = await _context.WeightLogs.SingleOrDefaultAsync(wl => wl.Id == logId && wl.UserId == userId);

            if (weightLog == null)
            {
                return null;
            }

            weightLog.LogDate = request.LogDate.ToUniversalTime();
            weightLog.Weight = request.Weight;
            weightLog.WaistSizeCm = request.WaistSizeCm;
            weightLog.BodyFatPercentage = request.BodyFatPercentage;
            weightLog.Notes = request.Notes;

            await _context.SaveChangesAsync();

            return new WeightLogDto
            {
                Id = weightLog.Id,
                LogDate = weightLog.LogDate,
                Weight = weightLog.Weight,
                WaistSizeCm = weightLog.WaistSizeCm,
                BodyFatPercentage = weightLog.BodyFatPercentage,
              
            };
        }

        public async Task<bool> DeleteWeightLogAsync(int userId, int logId)
        {
            var weightLog = await _context.WeightLogs.SingleOrDefaultAsync(wl => wl.Id == logId && wl.UserId == userId);

            if (weightLog == null)
            {
                return false;
            }

            _context.WeightLogs.Remove(weightLog);
            await _context.SaveChangesAsync();
            return true;
        }
    }
}
