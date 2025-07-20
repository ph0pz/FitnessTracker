using FitnessTracker.API.DTOs;
using FitnessTracker.API.DTOs.Request;
using FitnessTracker.API.Interfaces;
using FitnessTracker.API.Models;
using Microsoft.EntityFrameworkCore;

namespace FitnessTracker.API.Services
{
    public class ExerciseService : IExerciseService
    {
        private readonly FitnessTrackerDbContext _context;

        public ExerciseService(FitnessTrackerDbContext context)
        {
            _context = context;
        }

        public async Task<ExerciseEntryDto?> AddExerciseAsync(int userId, AddExerciseRequestDto request)
        {
            var exerciseEntry = new ExerciseEntry
            {
                UserId = userId,
                EntryDate = request.EntryDate.ToUniversalTime(), // Store UTC
                ExerciseName = request.ExerciseName,
                Sets = request.Sets,
                Reps = request.Reps,
                Weight = request.Weight,
                DurationMinutes = request.DurationMinutes,
                IsCompleted = false, // Newly added exercises are typically not yet completed
                Notes = request.Notes
            };

            _context.ExerciseEntries.Add(exerciseEntry);
            await _context.SaveChangesAsync();

            return new ExerciseEntryDto
            {
                Id = exerciseEntry.Id,
                EntryDate = exerciseEntry.EntryDate,
                ExerciseName = exerciseEntry.ExerciseName,
                Sets = exerciseEntry.Sets,
                Reps = exerciseEntry.Reps,
                Weight = exerciseEntry.Weight,
                DurationMinutes = exerciseEntry.DurationMinutes,
                IsCompleted = exerciseEntry.IsCompleted,
                Notes = exerciseEntry.Notes
            };
        }

        public async Task<List<ExerciseEntryDto>> GetExercisesByDateAsync(int userId, DateTime date)
        {
            var exercises = await _context.ExerciseEntries
                .Where(e => e.UserId == userId && e.EntryDate.Date == date.Date)
                .OrderBy(e => e.EntryDate)
                .Select(e => new ExerciseEntryDto
                {
                    Id = e.Id,
                    EntryDate = e.EntryDate,
                    ExerciseName = e.ExerciseName,
                    Sets = e.Sets,
                    Reps = e.Reps,
                    Weight = e.Weight,
                    DurationMinutes = e.DurationMinutes,
                    IsCompleted = e.IsCompleted,
                    Notes = e.Notes
                })
                .ToListAsync();

            return exercises;
        }

        public async Task<ExerciseEntryDto?> UpdateExerciseAsync(int userId, int exerciseId, AddExerciseRequestDto request)
        {
            var exerciseEntry = await _context.ExerciseEntries.SingleOrDefaultAsync(e => e.Id == exerciseId && e.UserId == userId);

            if (exerciseEntry == null)
            {
                return null;
            }

            exerciseEntry.EntryDate = request.EntryDate.ToUniversalTime();
            exerciseEntry.ExerciseName = request.ExerciseName;
            exerciseEntry.Sets = request.Sets;
            exerciseEntry.Reps = request.Reps;
            exerciseEntry.Weight = request.Weight;
            exerciseEntry.DurationMinutes = request.DurationMinutes;
            exerciseEntry.Notes = request.Notes;

            await _context.SaveChangesAsync();

            return new ExerciseEntryDto
            {
                Id = exerciseEntry.Id,
                EntryDate = exerciseEntry.EntryDate,
                ExerciseName = exerciseEntry.ExerciseName,
                Sets = exerciseEntry.Sets,
                Reps = exerciseEntry.Reps,
                Weight = exerciseEntry.Weight,
                DurationMinutes = exerciseEntry.DurationMinutes,
                IsCompleted = exerciseEntry.IsCompleted,
                Notes = exerciseEntry.Notes
            };
        }

        public async Task<bool> DeleteExerciseAsync(int userId, int exerciseId)
        {
            var exerciseEntry = await _context.ExerciseEntries.SingleOrDefaultAsync(e => e.Id == exerciseId && e.UserId == userId);

            if (exerciseEntry == null)
            {
                return false;
            }

            _context.ExerciseEntries.Remove(exerciseEntry);
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<bool> MarkExerciseAsCompletedAsync(int userId, int exerciseId, bool isCompleted)
        {
            var exerciseEntry = await _context.ExerciseEntries.SingleOrDefaultAsync(e => e.Id == exerciseId && e.UserId == userId);

            if (exerciseEntry == null)
            {
                return false;
            }

            exerciseEntry.IsCompleted = isCompleted;
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<ExerciseTemplateDto?> CreateExerciseTemplateAsync(int userId, CreateExerciseTemplateRequestDto request)
        {
            var template = new ExerciseTemplate
            {
                UserId = userId,
                TemplateName = request.TemplateName,
                Description = request.Description,
                CreatedAt = DateTime.UtcNow,
                TemplateExerciseDetails = request.Exercises?.Select(e => new TemplateExerciseDetail
                {
                    ExerciseName = e.ExerciseName,
                    DefaultSets = e.DefaultSets,
                    DefaultReps = e.DefaultReps,
                    DefaultWeight = e.DefaultWeight
                }).ToList() ?? new List<TemplateExerciseDetail>()
            };

            _context.ExerciseTemplates.Add(template);
            await _context.SaveChangesAsync();

            return new ExerciseTemplateDto
            {
                Id = template.Id,
                TemplateName = template.TemplateName,
                Description = template.Description,
                Exercises = template.TemplateExerciseDetails.Select(e => new TemplateExerciseDetailDto
                {
                    Id = e.Id,
                    ExerciseName = e.ExerciseName,
                    DefaultSets = e.DefaultSets,
                    DefaultReps = e.DefaultReps,
                    DefaultWeight = e.DefaultWeight
                }).ToList()
            };
        }

        public async Task<List<ExerciseTemplateDto>> GetExerciseTemplatesAsync(int userId)
        {
            var templates = await _context.ExerciseTemplates
                .Where(t => t.UserId == userId)
                .Include(t => t.TemplateExerciseDetails)
                .Select(t => new ExerciseTemplateDto
                {
                    Id = t.Id,
                    TemplateName = t.TemplateName,
                    Description = t.Description,
                    Exercises = t.TemplateExerciseDetails.Select(e => new TemplateExerciseDetailDto
                    {
                        Id = e.Id,
                        ExerciseName = e.ExerciseName,
                        DefaultSets = e.DefaultSets,
                        DefaultReps = e.DefaultReps,
                        DefaultWeight = e.DefaultWeight
                    }).ToList()
                })
                .ToListAsync();
            return templates;
        }

        public async Task<ExerciseTemplateDto?> GetExerciseTemplateByIdAsync(int userId, int templateId)
        {
            var template = await _context.ExerciseTemplates
                .Where(t => t.Id == templateId && t.UserId == userId)
                .Include(t => t.TemplateExerciseDetails)
                .Select(t => new ExerciseTemplateDto
                {
                    Id = t.Id,
                    TemplateName = t.TemplateName,
                    Description = t.Description,
                    Exercises = t.TemplateExerciseDetails.Select(e => new TemplateExerciseDetailDto
                    {
                        Id = e.Id,
                        ExerciseName = e.ExerciseName,
                        DefaultSets = e.DefaultSets,
                        DefaultReps = e.DefaultReps,
                        DefaultWeight = e.DefaultWeight
                    }).ToList()
                })
                .FirstOrDefaultAsync();

            return template;
        }

        public async Task<ExerciseTemplateDto?> UpdateExerciseTemplateAsync(int userId, int templateId, UpdateExerciseTemplateRequestDto request)
        {
            var template = await _context.ExerciseTemplates
                .Include(t => t.TemplateExerciseDetails) // Ensure exercises are loaded to track changes
                .SingleOrDefaultAsync(t => t.Id == templateId && t.UserId == userId);

            if (template == null)
            {
                return null;
            }

            template.TemplateName = request.TemplateName;
            template.Description = request.Description;

            // Handle exercises within the template:
            // Remove exercises not in the updated list
            var existingExerciseIds = new HashSet<int>(request.Exercises.Where(e => e.Id.HasValue).Select(e => e.Id!.Value));
            foreach (var existingDetail in template.TemplateExerciseDetails.ToList())
            {
                if (!existingExerciseIds.Contains(existingDetail.Id))
                {
                    _context.TemplateExerciseDetails.Remove(existingDetail);
                }
            }

            // Add or update exercises
            foreach (var detailDto in request.Exercises)
            {
                if (detailDto.Id.HasValue && detailDto.Id.Value != 0) // Existing exercise
                {
                    var existingDetail = template.TemplateExerciseDetails.FirstOrDefault(e => e.Id == detailDto.Id.Value);
                    if (existingDetail != null)
                    {
                        existingDetail.ExerciseName = detailDto.ExerciseName;
                        existingDetail.DefaultSets = detailDto.DefaultSets;
                        existingDetail.DefaultReps = detailDto.DefaultReps;
                        existingDetail.DefaultWeight = detailDto.DefaultWeight;
                    }
                }
                else // New exercise
                {
                    template.TemplateExerciseDetails.Add(new TemplateExerciseDetail
                    {
                        ExerciseName = detailDto.ExerciseName,
                        DefaultSets = detailDto.DefaultSets,
                        DefaultReps = detailDto.DefaultReps,
                        DefaultWeight = detailDto.DefaultWeight,
                        ExerciseTemplateId = template.Id // Ensure FK is set
                    });
                }
            }

            await _context.SaveChangesAsync();

            return await GetExerciseTemplateByIdAsync(userId, template.Id); // Fetch the updated template
        }

        public async Task<bool> DeleteExerciseTemplateAsync(int userId, int templateId)
        {
            var template = await _context.ExerciseTemplates.SingleOrDefaultAsync(t => t.Id == templateId && t.UserId == userId);

            if (template == null)
            {
                return false;
            }

            _context.ExerciseTemplates.Remove(template);
            await _context.SaveChangesAsync();
            return true;
        }
    }
}
