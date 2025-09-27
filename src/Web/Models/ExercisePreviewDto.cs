namespace Web.Models;

public class ExercisePreviewDto
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string? MuscleGroup { get; set; }
    public string? NormalizedMuscleGroup { get; set; }
    public string? Category { get; set; }
}
