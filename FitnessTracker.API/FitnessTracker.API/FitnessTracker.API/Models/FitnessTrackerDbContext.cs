using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;

namespace FitnessTracker.API.Models;

public partial class FitnessTrackerDbContext : DbContext
{
    public FitnessTrackerDbContext()
    {
    }

    public FitnessTrackerDbContext(DbContextOptions<FitnessTrackerDbContext> options)
        : base(options)
    {
    }

    public virtual DbSet<ExerciseEntry> ExerciseEntries { get; set; }

    public virtual DbSet<ExerciseTemplate> ExerciseTemplates { get; set; }

    public virtual DbSet<MacroGoal> MacroGoals { get; set; }

    public virtual DbSet<MealEntry> MealEntries { get; set; }

    public virtual DbSet<SavedMeal> SavedMeals { get; set; }

    public virtual DbSet<TemplateExerciseDetail> TemplateExerciseDetails { get; set; }

    public virtual DbSet<User> Users { get; set; }

    public virtual DbSet<UserProfile> UserProfiles { get; set; }

    public virtual DbSet<WeightLog> WeightLogs { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
#warning To protect potentially sensitive information in your connection string, you should move it out of source code. You can avoid scaffolding the connection string by using the Name= syntax to read it from configuration - see https://go.microsoft.com/fwlink/?linkid=2131148. For more guidance on storing connection strings, see https://go.microsoft.com/fwlink/?LinkId=723263.
        => optionsBuilder.UseSqlServer("Server=localhost,1433;Database=FitnessTrackerDb;User Id=sa;Password=yourStrong(!)Password;TrustServerCertificate=True;");

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<ExerciseEntry>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__Exercise__3214EC07E76CC1C1");

            entity.HasIndex(e => new { e.UserId, e.EntryDate }, "IX_ExerciseEntries_UserId_EntryDate").IsDescending(false, true);

            entity.Property(e => e.ExerciseName).HasMaxLength(100);
            entity.Property(e => e.Weight).HasColumnType("decimal(7, 2)");

            entity.HasOne(d => d.User).WithMany(p => p.ExerciseEntries)
                .HasForeignKey(d => d.UserId)
                .HasConstraintName("FK__ExerciseE__UserI__571DF1D5");
        });

        modelBuilder.Entity<ExerciseTemplate>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__Exercise__3214EC07AD73C403");

            entity.HasIndex(e => e.UserId, "IX_ExerciseTemplates_UserId");

            entity.Property(e => e.CreatedAt).HasDefaultValueSql("(getdate())");
            entity.Property(e => e.TemplateName).HasMaxLength(150);

            entity.HasOne(d => d.User).WithMany(p => p.ExerciseTemplates)
                .HasForeignKey(d => d.UserId)
                .HasConstraintName("FK__ExerciseT__UserI__5070F446");
        });

        modelBuilder.Entity<MacroGoal>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__MacroGoa__3214EC071AC2A4A2");

            entity.HasIndex(e => new { e.UserId, e.SetDate }, "IX_MacroGoals_UserId_SetDate").IsDescending(false, true);

            entity.Property(e => e.GoalType).HasMaxLength(20);
            entity.Property(e => e.IsActive).HasDefaultValue(true);
            entity.Property(e => e.SetDate).HasDefaultValueSql("(getdate())");

            entity.HasOne(d => d.User).WithMany(p => p.MacroGoals)
                .HasForeignKey(d => d.UserId)
                .HasConstraintName("FK__MacroGoal__UserI__46E78A0C");
        });

        modelBuilder.Entity<MealEntry>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__MealEntr__3214EC07D4D394D3");

            entity.HasIndex(e => new { e.UserId, e.EntryDate }, "IX_MealEntries_UserId_EntryDate").IsDescending(false, true);

            entity.Property(e => e.MealName).HasMaxLength(100);

            entity.HasOne(d => d.User).WithMany(p => p.MealEntries)
                .HasForeignKey(d => d.UserId)
                .HasConstraintName("FK__MealEntri__UserI__49C3F6B7");
        });

        modelBuilder.Entity<SavedMeal>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__SavedMea__3214EC0741069C7B");

            entity.HasIndex(e => e.UserId, "IX_SavedMeals_UserId");

            entity.Property(e => e.Name).HasMaxLength(150);

            entity.HasOne(d => d.User).WithMany(p => p.SavedMeals)
                .HasForeignKey(d => d.UserId)
                .HasConstraintName("FK__SavedMeal__UserI__4CA06362");
        });

        modelBuilder.Entity<TemplateExerciseDetail>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__Template__3214EC0781DDF00C");

            entity.Property(e => e.DefaultWeight).HasColumnType("decimal(7, 2)");
            entity.Property(e => e.ExerciseName).HasMaxLength(100);

            entity.HasOne(d => d.ExerciseTemplate).WithMany(p => p.TemplateExerciseDetails)
                .HasForeignKey(d => d.ExerciseTemplateId)
                .HasConstraintName("FK__TemplateE__Exerc__534D60F1");
        });

        modelBuilder.Entity<User>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__Users__3214EC07839B8FFC");

            entity.HasIndex(e => e.Email, "IX_Users_Email");

            entity.HasIndex(e => e.Username, "IX_Users_Username");

            entity.HasIndex(e => e.Username, "UQ__Users__536C85E45BB886E2").IsUnique();

            entity.HasIndex(e => e.Email, "UQ__Users__A9D10534352B7DA0").IsUnique();

            entity.Property(e => e.CreatedAt).HasDefaultValueSql("(getdate())");
            entity.Property(e => e.Email).HasMaxLength(100);
            entity.Property(e => e.PasswordHash).HasMaxLength(256);
            entity.Property(e => e.Username).HasMaxLength(50);
        });

        modelBuilder.Entity<UserProfile>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__UserProf__3214EC0761B64F2E");

            entity.HasIndex(e => e.UserId, "UQ__UserProf__1788CC4D8008FE65").IsUnique();

            entity.Property(e => e.DailyCalorieTarget).HasDefaultValue(2000);
            entity.Property(e => e.DailyCarbTarget).HasDefaultValue(200);
            entity.Property(e => e.DailyFatTarget).HasDefaultValue(60);
            entity.Property(e => e.DailyProteinTarget).HasDefaultValue(150);

            entity.HasOne(d => d.User).WithOne(p => p.UserProfile)
                .HasForeignKey<UserProfile>(d => d.UserId)
                .HasConstraintName("FK__UserProfi__UserI__412EB0B6");
        });

        modelBuilder.Entity<WeightLog>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__WeightLo__3214EC074034F261");

            entity.HasIndex(e => new { e.UserId, e.LogDate }, "IX_WeightLogs_UserId_LogDate").IsDescending(false, true);

            entity.Property(e => e.BodyFatPercentage).HasColumnType("decimal(4, 2)");
            entity.Property(e => e.WaistSizeCm).HasColumnType("decimal(5, 2)");
            entity.Property(e => e.Weight).HasColumnType("decimal(5, 2)");

            entity.HasOne(d => d.User).WithMany(p => p.WeightLogs)
                .HasForeignKey(d => d.UserId)
                .HasConstraintName("FK__WeightLog__UserI__59FA5E80");
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
