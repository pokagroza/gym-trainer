namespace Infrastructure.External;

public class ExternalWorkoutCacheOptions
{
    public const string SectionName = "ExternalWorkoutCache";
    public int ListTtlSeconds { get; set; } = 60;
    public int ItemTtlSeconds { get; set; } = 120;
}
