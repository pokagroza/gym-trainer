using Domain.Entities;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Application.Services;
using System.Collections.Generic;
using System.Linq;

public class HistoryModel : PageModel
{
    private readonly ITrainingHistoryService _historyService;
    public List<TrainingHistory>? History { get; set; }

    public HistoryModel(ITrainingHistoryService historyService)
    {
        _historyService = historyService;
    }

    public IActionResult OnGet()
    {
        var userId = HttpContext.Session.GetInt32("UserId");
        if (userId == null)
        {
            History = null;
            return Page();
        }
        History = _historyService.GetUserHistory(userId.Value).ToList();
        return Page();
    }
}
