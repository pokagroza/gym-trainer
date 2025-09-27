using Infrastructure.Persistence;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Web.Models;

namespace Web;

[ApiController]
[Route("api/[controller]")]
public class ExercisesController : ControllerBase
{
    private readonly GymDbContext _db;
    public ExercisesController(GymDbContext db) => _db = db;

    /// <summary>
    /// Returns a lightweight list of exercises with optional filters.
    /// </summary>
    /// <param name="muscleGroup">Filter by normalized muscle group (preferred) or original muscle group (case-insensitive).</param>
    /// <param name="search">Substring filter on name (case-insensitive).</param>
    /// <param name="skip">Records to skip (pagination).</param>
    /// <param name="take">Records to take (default 50, max 200).</param>
    [HttpGet("preview")]
    public ActionResult<IEnumerable<ExercisePreviewDto>> GetPreview(
        [FromQuery] string? muscleGroup,
        [FromQuery] string? search,
        [FromQuery] int skip = 0,
        [FromQuery] int take = 50)
    {
        if (take <= 0) take = 50;
        if (take > 200) take = 200;
        if (skip < 0) skip = 0;

        var q = _db.Exercises
            .AsNoTracking()
            .Include(e => e.Category)
            .Select(e => new ExercisePreviewDto
            {
                Id = e.Id,
                Name = e.Name,
                MuscleGroup = e.MuscleGroup,
                NormalizedMuscleGroup = e.NormalizedMuscleGroup,
                Category = e.Category != null ? e.Category.Name : null
            });

        if (!string.IsNullOrWhiteSpace(muscleGroup))
        {
            var mg = muscleGroup.Trim().ToLowerInvariant();
            q = q.Where(e => (e.NormalizedMuscleGroup != null && e.NormalizedMuscleGroup.ToLower() == mg) || e.MuscleGroup.ToLower() == mg);
        }

        if (!string.IsNullOrWhiteSpace(search))
        {
            var s = search.Trim().ToLowerInvariant();
            q = q.Where(e => e.Name.ToLower().Contains(s));
        }

        var results = q
            .OrderBy(e => e.Name)
            .Skip(skip)
            .Take(take)
            .ToList();

        return Ok(results);
    }
}
