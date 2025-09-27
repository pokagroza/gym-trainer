namespace Web.Models;

public record CategoryDto(int Id, string Name, string? Description, int ExerciseCount);
public record CategoryCreateUpdateDto(
	[property: System.ComponentModel.DataAnnotations.Required]
	[property: System.ComponentModel.DataAnnotations.MinLength(2)]
	string Name,
	[property: System.ComponentModel.DataAnnotations.MaxLength(500)]
	string? Description
);
