using Application.Services;
using Infrastructure.Persistence;
using Microsoft.AspNetCore.Mvc;

namespace Web.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class TrainingController : ControllerBase
    {
        private readonly ITrainingOptimizerService _optimizerService;
        public TrainingController(ITrainingOptimizerService optimizerService)
        {
            _optimizerService = optimizerService;
        }

        [HttpGet("optimize/{userId}")]
        public IActionResult Optimize(int userId)
        {
            try
            {
                var split = _optimizerService.GetOptimizedSplit(userId);
                if (split == null) return NotFound();
                return Ok(split);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Ошибка при оптимизации: {ex.Message}");
            }
        }
        [HttpGet("all-splits")]
        public IActionResult AllSplits()
        {
            try
            {
                return Ok(_optimizerService.GetAllSplits());
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Ошибка при получении сплитов: {ex.Message}");
            }
        }

        [HttpGet("optimize-by-params")]
        public IActionResult OptimizeByParams([FromQuery] int fatigue, [FromQuery] int experience)
        {
            try
            {
                var split = _optimizerService.GetOptimizedSplitByParams(fatigue, experience);
                if (split == null) return NotFound();
                return Ok(split);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Ошибка при оптимизации по параметрам: {ex.Message}");
            }
        }

        // Returns multiple optimized suggestions based on parameters
        [HttpGet("suggestions")]
        public IActionResult Suggestions([FromQuery] int fatigue, [FromQuery] int experience, [FromQuery] string? difficulty, [FromQuery] int limit = 3)
        {
            try
            {
                if (limit <= 0) limit = 1;
                if (limit > 10) limit = 10; // sanity cap
                var list = _optimizerService.GetOptimizedSplitsByParams(fatigue, experience, difficulty, limit);
                // Project to DTO to avoid circular references (TrainingSplit -> SplitExercise -> TrainingSplit)
                var dto = list.Select(s => new
                {
                    s.Id,
                    s.Name,
                    s.Description,
                    Difficulty = s.Difficulty.ToString(),
                    Exercises = (s.Exercises == null
                        ? new List<object>()
                        : s.Exercises.Select(se => (object)new
                        {
                            se.Id,
                            se.Sets,
                            se.Reps,
                            ExerciseId = se.ExerciseId,
                            ExerciseName = se.Exercise.Name,
                            se.Exercise.MuscleGroup,
                            se.Exercise.Equipment
                        }).ToList())
                });
                return Ok(dto);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Ошибка при получении предложений: {ex.Message}\n{ex.StackTrace}");
            }
        }
    }
}