namespace Domain.Entities;

public class SplitExercise
{
    public int Id { get; set; }
    public int TrainingSplitId { get; set; }
    public int ExerciseId { get; set; }
    public int Sets { get; set; }
    public int Reps { get; set; }
    public TrainingSplit TrainingSplit { get; set; } = null!;
    public Exercise Exercise { get; set; } = null!;
}
