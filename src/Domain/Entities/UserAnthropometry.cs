namespace Domain.Entities;

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
