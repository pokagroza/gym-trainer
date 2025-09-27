using Domain.Entities;

namespace Web.Models;

public static class CategoryMappers
{
    public static CategoryDto ToDto(this ExerciseCategory c)
        => new(c.Id, c.Name, c.Description, c.Exercises?.Count ?? 0);

    public static void Apply(this ExerciseCategory entity, CategoryCreateUpdateDto dto)
    {
        entity.Name = dto.Name.Trim();
        entity.Description = dto.Description?.Trim();
    }
}
