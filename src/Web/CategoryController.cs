using Application.Repositories;
using Domain.Entities;
using Microsoft.AspNetCore.Mvc;
using Web.Models;

namespace Web;

[ApiController]
[Route("api/categories")]
public class CategoryController : ControllerBase
{
    private readonly ICategoryRepository _repo;
    public CategoryController(ICategoryRepository repo) => _repo = repo;

    [HttpGet]
    public ActionResult<IEnumerable<CategoryDto>> GetAll()
        => Ok(_repo.GetAll().Select(c => c.ToDto()));

    [HttpGet("{id}")]
    public ActionResult<CategoryDto> Get(int id)
    {
        var c = _repo.GetById(id);
        if (c == null) return NotFound();
        return Ok(c.ToDto());
    }

    [HttpPost]
    public ActionResult<CategoryDto> Create([FromBody] CategoryCreateUpdateDto dto)
    {
        var entity = new ExerciseCategory { Name = dto.Name.Trim(), Description = dto.Description?.Trim() };
        _repo.Add(entity);
        _repo.Save();
        return CreatedAtAction(nameof(Get), new { id = entity.Id }, entity.ToDto());
    }

    [HttpPut("{id}")]
    public IActionResult Update(int id, [FromBody] CategoryCreateUpdateDto dto)
    {
        var existing = _repo.GetById(id);
        if (existing == null) return NotFound();
        existing.Apply(dto);
        _repo.Update(existing);
        _repo.Save();
        return NoContent();
    }

    [HttpDelete("{id}")]
    public IActionResult Delete(int id)
    {
        _repo.Delete(id);
        _repo.Save();
        return NoContent();
    }
}
