namespace Domain.Entities
{
    public class User
    {
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public int Age { get; set; }
    public double Height { get; set; } // cm
    public double Weight { get; set; } // kg
    public double FatigueLevel { get; set; } // 0-10
    public ICollection<UserAnthropometry> Anthropometries { get; set; } = new List<UserAnthropometry>();
    }

    public class UserAnthropometry
    {
    public int Id { get; set; }
    public int UserId { get; set; }
    public double Chest { get; set; }
    public double Waist { get; set; }
    public double Hip { get; set; }
    public double Arm { get; set; }
    public double Leg { get; set; }
    public User User { get; set; } = null!;
    }

    public class Exercise
    {
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string MuscleGroup { get; set; } = string.Empty;
    public string Equipment { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    }

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
}