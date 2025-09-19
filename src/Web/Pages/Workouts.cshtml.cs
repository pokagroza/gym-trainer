using Domain.Entities;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Application.Services;
using System.Collections.Generic;

public class WorkoutsModel : PageModel
{
    private readonly TrainingOptimizerService _optimizerService;
    private readonly TrainingHistoryService _historyService;
    public List<TrainingSplit>? Splits { get; set; }

    public WorkoutsModel(TrainingOptimizerService optimizerService, TrainingHistoryService historyService)
    {
        _optimizerService = optimizerService;
        _historyService = historyService;
    }

    public IActionResult OnGet([FromQuery] string difficulty = "all")
    {
        var all = _optimizerService.GetAllSplits();
        if (difficulty == "easy") Splits = all.Where(s => s.Difficulty == Domain.Entities.Difficulty.Easy).ToList();
        else if (difficulty == "hard") Splits = all.Where(s => s.Difficulty == Domain.Entities.Difficulty.Hard).ToList();
        else if (difficulty == "medium") Splits = all.Where(s => s.Difficulty == Domain.Entities.Difficulty.Medium).ToList();
        else Splits = all;
        return Page();
    }

    public IActionResult OnPost([FromForm] int splitId)
    {
        var userId = HttpContext.Session.GetInt32("UserId");
        if (userId == null) return RedirectToPage("/Login");
        _historyService.AddHistory(userId.Value, splitId, "Отметил тренировку через UI");
        return RedirectToPage();
    }
}
