using Domain.Entities;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Application.Services;

public class OptimizeModel : PageModel
{
    private readonly ITrainingOptimizerService _optimizerService;
    private readonly ITrainingHistoryService _historyService;

    [BindProperty] public int Fatigue { get; set; }
    [BindProperty] public int Experience { get; set; }
    [BindProperty] public string? Difficulty { get; set; }
    public TrainingSplit? Split { get; set; }
    public List<TrainingSplit> Suggestions { get; set; } = new List<TrainingSplit>();

    [BindProperty]
    public int? SelectedSplitId { get; set; }

    public OptimizeModel(ITrainingOptimizerService optimizerService, ITrainingHistoryService historyService)
    {
        _optimizerService = optimizerService;
        _historyService = historyService;
    }

    public IActionResult OnGet([FromQuery] string? difficulty = null, [FromQuery] int? fatigue = null, [FromQuery] int? experience = null)
    {
        var userId = HttpContext.Session.GetInt32("UserId");
        if (userId == null)
        {
            Split = null;
            return Page();
        }
        if (fatigue.HasValue) Fatigue = fatigue.Value;
        if (experience.HasValue) Experience = experience.Value;
        Difficulty = difficulty;
        // If parameters are provided, compute optimized split
        if (fatigue.HasValue || experience.HasValue || !string.IsNullOrEmpty(difficulty))
        {
            Suggestions = _optimizerService.GetOptimizedSplitsByParams(Fatigue, Experience, difficulty, 3);
            Split = Suggestions.FirstOrDefault();
        }
        return Page();
    }

    public IActionResult OnPost()
    {
        var userId = HttpContext.Session.GetInt32("UserId");
        if (userId == null)
        {
            Split = null;
            return Page();
        }
        // Persist selection in query string so page can be bookmarked/shared
        var qs = $"?fatigue={Fatigue}&experience={Experience}" + (string.IsNullOrEmpty(Difficulty) ? string.Empty : $"&difficulty={Difficulty}");
        return Redirect("/Optimize" + qs);
    }

    // Handler when user selects a suggested split
    public IActionResult OnPostSelect()
    {
        var userId = HttpContext.Session.GetInt32("UserId");
        if (userId == null || SelectedSplitId == null) return RedirectToPage();
        // Save to history
        _historyService.AddHistory(userId.Value, SelectedSplitId.Value, "Выбрана рекомендованная тренировка");
        return RedirectToPage("/Workouts");
    }
}
