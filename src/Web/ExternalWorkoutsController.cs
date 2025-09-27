using Application.Services;
using Microsoft.AspNetCore.Mvc;

namespace Web.Controllers;

[ApiController]
[Route("api/external-workouts")]
public class ExternalWorkoutsController : ControllerBase
{
    private readonly IExternalWorkoutSyncService _syncService;
    public ExternalWorkoutsController(IExternalWorkoutSyncService syncService)
    {
        _syncService = syncService;
    }

    [HttpGet("preview")] // GET api/external-workouts/preview?page=1&pageSize=10
    public async Task<IActionResult> Preview([FromQuery] int page = 1, [FromQuery] int pageSize = 10, CancellationToken ct = default)
    {
        if (page < 1) page = 1;
        if (pageSize < 1) pageSize = 1;
        if (pageSize > 50) pageSize = 50;
        var workouts = await _syncService.PreviewAsync(page, pageSize, ct);
        return Ok(new { page, pageSize, count = workouts.Count, items = workouts });
    }

    [HttpPost("import/{externalId}")] // POST api/external-workouts/import/abc123?overwriteExisting=true
    public async Task<IActionResult> Import(string externalId, [FromQuery] bool overwriteExisting = false, CancellationToken ct = default)
    {
        if (string.IsNullOrWhiteSpace(externalId)) return BadRequest("externalId required");
        var id = await _syncService.ImportAsync(externalId, overwriteExisting, ct);
        if (id == -1) return NotFound();
        return Ok(new { localSplitId = id });
    }
}