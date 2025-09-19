using Application.Services;
using Infrastructure.Persistence;
using Microsoft.AspNetCore.Mvc;

namespace Web.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class HistoryController : ControllerBase
    {
        private readonly ITrainingHistoryService _historyService;
        public HistoryController(ITrainingHistoryService historyService)
        {
            _historyService = historyService;
        }

        [HttpPost]
        public IActionResult Add([FromBody] HistoryDto dto)
        {
            if (dto.UserId <= 0 || dto.TrainingSplitId <= 0)
                return BadRequest("UserId и TrainingSplitId должны быть положительными.");
            try
            {
                _historyService.AddHistory(dto.UserId, dto.TrainingSplitId, dto.Notes ?? string.Empty);
                return Ok(new { message = "История добавлена" });
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Ошибка при добавлении истории: {ex.Message}");
            }
        }

        [HttpGet("{userId}")]
        public ActionResult<IEnumerable<object>> Get(int userId)
        {
            if (userId <= 0)
                return BadRequest("Некорректный userId.");
            try
            {
                var history = _historyService.GetUserHistory(userId);
                if (history == null || !history.Any())
                    return NotFound("История не найдена.");
                return Ok(history);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Ошибка при получении истории: {ex.Message}");
            }
        }
    }

    public class HistoryDto
    {
        public int UserId { get; set; }
        public int TrainingSplitId { get; set; }
        public string Notes { get; set; } = string.Empty;
    }
}