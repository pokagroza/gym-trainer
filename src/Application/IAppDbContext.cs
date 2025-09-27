using Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;

namespace Application.Abstractions;

public interface IAppDbContext
{
    DbSet<User> Users { get; }
    DbSet<UserAnthropometry> UserAnthropometries { get; }
    DbSet<Exercise> Exercises { get; }
    DbSet<ExerciseCategory> ExerciseCategories { get; }
    DbSet<TrainingSplit> TrainingSplits { get; }
    DbSet<SplitExercise> SplitExercises { get; }
    DbSet<TrainingHistory> TrainingHistories { get; }
    DbSet<UserAuth> UserAuths { get; }

    int SaveChanges();
    DatabaseFacade Database { get; }
}
