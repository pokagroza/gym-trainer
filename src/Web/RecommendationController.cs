using Application.Services;
using Microsoft.AspNetCore.Mvc;

namespace Web.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class RecommendationController : ControllerBase
    {
        private readonly RecommendationService _service;
        public RecommendationController(RecommendationService service)
        {
            _service = service;
        }

        [HttpGet("exercises/{userId}")]
        public IActionResult RecommendExercises(int userId)
        {
            try
            {
                var result = _service.RecommendExercises(userId);
                return Ok(result);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Ошибка при получении рекомендаций: {ex.Message}");
            }
        }
    }
}