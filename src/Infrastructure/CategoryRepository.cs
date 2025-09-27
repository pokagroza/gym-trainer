using Application.Repositories;
using Application.Abstractions;
using Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Repositories;

public class CategoryRepository : ICategoryRepository
{
    private readonly IAppDbContext _context;
    public CategoryRepository(IAppDbContext context) => _context = context;

    public IEnumerable<ExerciseCategory> GetAll()
        => _context.ExerciseCategories.Include(c => c.Exercises).AsNoTracking().ToList();

    public ExerciseCategory? GetById(int id)
        => _context.ExerciseCategories.Include(c => c.Exercises).FirstOrDefault(c => c.Id == id);

    public void Add(ExerciseCategory category)
    {
        _context.ExerciseCategories.Add(category);
    }

    public void Update(ExerciseCategory category)
    {
        _context.ExerciseCategories.Update(category);
    }

    public void Delete(int id)
    {
        var entity = _context.ExerciseCategories.FirstOrDefault(c => c.Id == id);
        if (entity != null)
        {
            _context.ExerciseCategories.Remove(entity);
        }
    }

    public void Save() => _context.SaveChanges();
}
