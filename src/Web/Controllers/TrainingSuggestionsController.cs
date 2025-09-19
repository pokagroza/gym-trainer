using Application.Services;
using Microsoft.AspNetCore.Mvc;

namespace Web.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class TrainingSuggestionsController : ControllerBase
    {
        private readonly TrainingOptimizerService _optimizer;
        public TrainingSuggestionsController(TrainingOptimizerService optimizer)
        {
            _optimizer = optimizer;
        }

        [HttpGet]
        public IActionResult Get([FromQuery] int fatigue = 5, [FromQuery] int experience = 1, [FromQuery] string? difficulty = null, [FromQuery] int n = 3)
        {
            var list = _optimizer.GetOptimizedSplitsByParams(fatigue, experience, difficulty, n);
            // Return simplified DTO to avoid circular refs
            var result = list.Select(s => new {
                s.Id,
                s.Name,
                s.Description,
                Difficulty = s.Difficulty.ToString(),
                Exercises = s.Exercises.Select(e => new { e.ExerciseId, Name = e.Exercise.Name, e.Sets, e.Reps })
            });
            return Ok(result);
        }
    }
}
