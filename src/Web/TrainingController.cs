using Application.Services;
using Infrastructure.Persistence;
using Microsoft.AspNetCore.Mvc;
using Web.Models;

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
            var split = _optimizerService.GetOptimizedSplit(userId);
            if (split == null) return NotFound();
            return Ok(split.ToDto());
        }
        [HttpGet("all-splits")]
        public IActionResult AllSplits()
        {
            return Ok(_optimizerService.GetAllSplits().Select(s => s.ToDto()));
        }

        [HttpGet("optimize-by-params")]
        public IActionResult OptimizeByParams([FromQuery] int fatigue, [FromQuery] int experience)
        {
            var split = _optimizerService.GetOptimizedSplitByParams(fatigue, experience);
            if (split == null) return NotFound();
            return Ok(split.ToDto());
        }

        // Returns multiple optimized suggestions based on parameters
        [HttpGet("suggestions")]
        public IActionResult Suggestions([FromQuery] int fatigue, [FromQuery] int experience, [FromQuery] string? difficulty, [FromQuery] int limit = 3)
        {
            if (limit <= 0) limit = 1;
            if (limit > 10) limit = 10; // sanity cap
            var list = _optimizerService.GetOptimizedSplitsByParams(fatigue, experience, difficulty, limit);
            return Ok(list.Select(s => s.ToDto()));
        }
    }
}