namespace Web.Models;

public record ExerciseSummaryDto(int Id, string Name, string MuscleGroup, string? Equipment);
public record SplitExerciseDto(int Id, int ExerciseId, string ExerciseName, string MuscleGroup, string? Equipment, int Sets, int Reps);
public record TrainingSplitDto(int Id, string Name, string? Description, string Difficulty, IReadOnlyCollection<SplitExerciseDto> Exercises);

public static class TrainingDtoMappers
{
    public static TrainingSplitDto ToDto(this Domain.Entities.TrainingSplit split)
    {
        var exercises = (split.Exercises ?? new List<Domain.Entities.SplitExercise>())
            .Select(se => new SplitExerciseDto(
                se.Id,
                se.ExerciseId,
                se.Exercise?.Name ?? string.Empty,
                se.Exercise?.MuscleGroup ?? string.Empty,
                se.Exercise?.Equipment,
                se.Sets,
                se.Reps
            )).ToList();
        return new TrainingSplitDto(
            split.Id,
            split.Name,
            split.Description,
            split.Difficulty.ToString(),
            exercises
        );
    }

    public static IEnumerable<TrainingSplitDto> ToDtoList(this IEnumerable<Domain.Entities.TrainingSplit> splits)
        => splits.Select(s => s.ToDto());
}