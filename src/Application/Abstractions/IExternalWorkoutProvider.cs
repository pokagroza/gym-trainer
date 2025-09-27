namespace Application.Abstractions;

public record ExternalExerciseDto(string ExternalId, string Name, string MuscleGroup, string? Equipment, string? Instructions);
public record ExternalWorkoutDto(string ExternalId, string Title, string? Description, IReadOnlyCollection<ExternalExerciseDto> Exercises);

public interface IExternalWorkoutProvider
{
    /// <summary>
    /// Returns a page of external workouts.
    /// </summary>
    /// <param name="page">1-based page index</param>
    /// <param name="pageSize">Items per page (max 50 enforced by implementation).</param>
    Task<IReadOnlyCollection<ExternalWorkoutDto>> GetWorkoutsAsync(int page = 1, int pageSize = 10, CancellationToken ct = default);
    Task<ExternalWorkoutDto?> GetWorkoutByIdAsync(string externalId, CancellationToken ct = default);
}