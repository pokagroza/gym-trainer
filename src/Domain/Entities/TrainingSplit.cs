namespace Domain.Entities;

public enum Difficulty
{
    Easy = 0,
    Medium = 1,
    Hard = 2
}

public class TrainingSplit
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public Difficulty Difficulty { get; set; } = Difficulty.Medium;
    public ICollection<SplitExercise> Exercises { get; set; } = new List<SplitExercise>();
}
