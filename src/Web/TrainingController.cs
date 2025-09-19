using Application.Services;
using Infrastructure.Persistence;
using Microsoft.AspNetCore.Mvc;

namespace Web.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class TrainingController : ControllerBase
    {
        private readonly TrainingOptimizerService _optimizerService;
        public TrainingController(TrainingOptimizerService optimizerService)
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
    }
}