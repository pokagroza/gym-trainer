using Domain.Entities;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Application.Services;
using System.Collections.Generic;
using System.Linq;

public class RecommendationsModel : PageModel
{
    private readonly RecommendationService _recommendationService;
    public List<Exercise>? Exercises { get; set; }

    public RecommendationsModel(RecommendationService recommendationService)
    {
        _recommendationService = recommendationService;
    }

    public IActionResult OnGet()
    {
        var userId = HttpContext.Session.GetInt32("UserId");
        if (userId == null)
        {
            Exercises = null;
            return Page();
        }
        Exercises = _recommendationService.RecommendExercises(userId.Value).ToList();
        return Page();
    }
}
