using Domain.Entities;

namespace Web.Models;

public enum SplitCardActionType
{
    None,
    LoginToSelect,
    MarkInHistory,
    SelectSplit
}

public class SplitCardModel
{
    public TrainingSplit Split { get; set; } = null!;
    public bool ShowExercises { get; set; } = true;
    public int ExercisePreviewCount { get; set; } = 4;
    public SplitCardActionType ActionType { get; set; } = SplitCardActionType.None;
    public bool IsAuthenticated { get; set; }
        = false;
    // Used for forms
    public string? PostHandler { get; set; } // for Optimize page select ("Select")
    public string? PostUrl { get; set; } // fallback action URL (e.g. Workouts)
}