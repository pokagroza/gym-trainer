namespace Domain.Entities;

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
