using Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Persistence
{
    public class GymDbContext : DbContext
    {
        public GymDbContext(DbContextOptions<GymDbContext> options) : base(options) { }

        public DbSet<User> Users { get; set; }
        public DbSet<UserAnthropometry> UserAnthropometries { get; set; }
        public DbSet<Exercise> Exercises { get; set; }
        public DbSet<TrainingSplit> TrainingSplits { get; set; }
        public DbSet<SplitExercise> SplitExercises { get; set; }

        public DbSet<TrainingHistory> TrainingHistories { get; set; }

    public DbSet<UserAuth> UserAuths { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
            // Fluent API configs
            modelBuilder.Entity<UserAuth>()
                .HasIndex(u => u.Username)
                .IsUnique();
        }
    }
}