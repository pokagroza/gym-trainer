using Application.Services;
using Microsoft.AspNetCore.Mvc;
using Web.Models;

namespace Web.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class RecommendationController : ControllerBase
    {
        private readonly IRecommendationService _service;
        public RecommendationController(IRecommendationService service)
        {
            _service = service;
        }

        [HttpGet("exercises/{userId}")]
        public IActionResult RecommendExercises(int userId)
        {
            var result = _service.RecommendExercises(userId)
                .Select(e => new ExerciseSummaryDto(e.Id, e.Name, e.MuscleGroup, e.Equipment));
            return Ok(result);
        }
    }
}