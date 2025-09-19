namespace Domain.Entities
{
    public class TrainingHistory
    {
    public int Id { get; set; }
    public int UserId { get; set; }
    public DateTime Date { get; set; }
    public int TrainingSplitId { get; set; }
    public string Notes { get; set; } = string.Empty;
    public User User { get; set; } = null!;
    public TrainingSplit TrainingSplit { get; set; } = null!;
    }
}