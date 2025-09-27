using Application.Repositories;
using Domain.Entities;
using Infrastructure.Persistence;
using Infrastructure.Repositories;
using Microsoft.EntityFrameworkCore;
using Xunit;

namespace Application.Tests;

public class CategoryRepositoryTests
{
    private (GymDbContext ctx, ICategoryRepository repo) Create()
    {
        var opts = new DbContextOptionsBuilder<GymDbContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString())
            .Options;
        var ctx = new GymDbContext(opts);
        return (ctx, new CategoryRepository(ctx));
    }

    [Fact]
    public void Add_And_GetById_Works()
    {
        var (ctx, repo) = Create();
        var cat = new ExerciseCategory { Name = "Грудь" };
        repo.Add(cat);
        repo.Save();
        Assert.True(cat.Id > 0);
        var loaded = repo.GetById(cat.Id);
        Assert.NotNull(loaded);
        Assert.Equal("Грудь", loaded!.Name);
    }

    [Fact]
    public void Update_Works()
    {
        var (ctx, repo) = Create();
        var cat = new ExerciseCategory { Name = "Спина" };
        repo.Add(cat); repo.Save();
        cat.Name = "Спина +";
        repo.Update(cat); repo.Save();
        var loaded = repo.GetById(cat.Id);
        Assert.Equal("Спина +", loaded!.Name);
    }

    [Fact]
    public void Delete_Works()
    {
        var (ctx, repo) = Create();
        var cat = new ExerciseCategory { Name = "Ноги" };
        repo.Add(cat); repo.Save();
        repo.Delete(cat.Id); repo.Save();
        Assert.Null(repo.GetById(cat.Id));
    }

    [Fact]
    public void GetAll_Returns_All()
    {
        var (ctx, repo) = Create();
        repo.Add(new ExerciseCategory { Name = "A" });
        repo.Add(new ExerciseCategory { Name = "B" });
        repo.Save();
        var all = repo.GetAll().ToList();
        Assert.Equal(2, all.Count);
    }
}
