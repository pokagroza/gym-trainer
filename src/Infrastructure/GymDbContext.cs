using Domain.Entities;
using Application.Abstractions;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Persistence
{
    public class GymDbContext : DbContext, IAppDbContext
    {
        public GymDbContext(DbContextOptions<GymDbContext> options) : base(options) { }

        public DbSet<User> Users { get; set; }
        public DbSet<UserAnthropometry> UserAnthropometries { get; set; }
        public DbSet<Exercise> Exercises { get; set; }
    public DbSet<ExerciseCategory> ExerciseCategories { get; set; }
        public DbSet<TrainingSplit> TrainingSplits { get; set; }
        public DbSet<SplitExercise> SplitExercises { get; set; }

        public DbSet<TrainingHistory> TrainingHistories { get; set; }

    public DbSet<UserAuth> UserAuths { get; set; }

    Microsoft.EntityFrameworkCore.Infrastructure.DatabaseFacade IAppDbContext.Database => base.Database;

    protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
            // Fluent API configs
            modelBuilder.Entity<UserAuth>()
                .HasIndex(u => u.Username)
                .IsUnique();

            // Exercise indexes (added via new migration):
            modelBuilder.Entity<Exercise>()
                .HasIndex(e => e.ExternalExerciseId)
                .IsUnique();

            modelBuilder.Entity<Exercise>()
                .HasIndex(e => e.NormalizedMuscleGroup);
        }
    }
}