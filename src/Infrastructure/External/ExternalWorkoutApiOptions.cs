namespace Infrastructure.External;

public class ExternalWorkoutApiOptions
{
    public const string SectionName = "ExternalWorkoutApi";
    public string BaseUrl { get; set; } = string.Empty;
    public string ApiKey { get; set; } = string.Empty; // if needed
    public int TimeoutSeconds { get; set; } = 10;
}