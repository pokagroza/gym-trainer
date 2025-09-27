namespace Domain.Entities;

public class Exercise
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string MuscleGroup { get; set; } = string.Empty;
    // Normalized (canonical) muscle group for grouping/reporting
    public string? NormalizedMuscleGroup { get; set; }
    public string Equipment { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public string Instructions { get; set; } = string.Empty;
    // External identifier (nullable) for deduplication across imports
    public string? ExternalExerciseId { get; set; }
    public int? ExerciseCategoryId { get; set; }
    public ExerciseCategory? Category { get; set; }
}
