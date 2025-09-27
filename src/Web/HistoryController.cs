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
            _historyService.AddHistory(dto.UserId, dto.TrainingSplitId, dto.Notes ?? string.Empty);
            return Ok(new { message = "История добавлена" });
        }

        [HttpGet("{userId}")]
        public ActionResult<IEnumerable<object>> Get(int userId)
        {
            if (userId <= 0)
                return BadRequest("Некорректный userId.");
            var history = _historyService.GetUserHistory(userId);
            if (history == null || !history.Any())
                return NotFound("История не найдена.");
            return Ok(history);
        }
    }

    public class HistoryDto
    {
        [System.ComponentModel.DataAnnotations.Range(1,int.MaxValue)]
        public int UserId { get; set; }
        [System.ComponentModel.DataAnnotations.Range(1,int.MaxValue)]
        public int TrainingSplitId { get; set; }
        [System.ComponentModel.DataAnnotations.MaxLength(1000)]
        public string Notes { get; set; } = string.Empty;
    }
}