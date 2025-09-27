using Application.Services;
using Microsoft.AspNetCore.DataProtection;
using Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;
using System.IO;
using Microsoft.Extensions.Hosting;
using Application.Repositories;
using Infrastructure.Repositories;
using Web.Middleware;
using Microsoft.AspNetCore.Mvc;
using Web.Models;

var builder = WebApplication.CreateBuilder(args);

// Resolve the SQLite file path relative to the app content root so the bundled test DB is used reliably
var dbPath = Path.Combine(builder.Environment.ContentRootPath, "gym.db");
// Add services
builder.Services.AddDbContext<GymDbContext>(options =>
    options.UseSqlite($"Data Source={dbPath}"));
builder.Services.AddScoped<Application.Abstractions.IAppDbContext>(sp => sp.GetRequiredService<GymDbContext>());
builder.Services.AddScoped<ITrainingOptimizerService, TrainingOptimizerService>();
builder.Services.AddScoped<ITrainingHistoryService, TrainingHistoryService>();
builder.Services.AddScoped<IAuthService, AuthService>();
builder.Services.AddScoped<IRecommendationService, RecommendationService>();
builder.Services.AddSingleton<Application.Abstractions.IRandomProvider, Infrastructure.RandomProvider>();
builder.Services.AddScoped<Application.Abstractions.IUnitOfWork, Infrastructure.Persistence.UnitOfWork>();
builder.Services.AddScoped<IExternalWorkoutSyncService, ExternalWorkoutSyncService>();
builder.Services.AddSingleton<IImportMetricsService, ImportMetricsService>();
builder.Services.AddScoped<ICategoryRepository, CategoryRepository>();
builder.Services.AddScoped<IUserRepository, UserRepository>();
builder.Services.AddScoped<IUserAuthRepository, UserAuthRepository>();
builder.Services.AddScoped<ITrainingSplitRepository, TrainingSplitRepository>();
builder.Services.AddScoped<ITrainingHistoryRepository, TrainingHistoryRepository>();
builder.Services.AddScoped<IExerciseRepository, ExerciseRepository>();
builder.Services.AddScoped<ISplitExerciseRepository, SplitExerciseRepository>();
builder.Services.AddControllers().ConfigureApiBehaviorOptions(o =>
{
    o.InvalidModelStateResponseFactory = context =>
    {
        var errors = context.ModelState
            .Where(kv => kv.Value?.Errors.Count > 0)
            .Select(kv => new { field = kv.Key, messages = kv.Value!.Errors.Select(e => e.ErrorMessage) });
        return new BadRequestObjectResult(new { message = "Validation failed", errors });
    };
});
builder.Services.AddRazorPages();
// External workout API integration
builder.Services.Configure<Infrastructure.External.ExternalWorkoutApiOptions>(builder.Configuration.GetSection(Infrastructure.External.ExternalWorkoutApiOptions.SectionName));
builder.Services.Configure<Infrastructure.External.ExternalWorkoutCacheOptions>(builder.Configuration.GetSection(Infrastructure.External.ExternalWorkoutCacheOptions.SectionName));
builder.Services.AddMemoryCache();
builder.Services.AddHttpClient<Infrastructure.External.HttpExternalWorkoutProvider>();
builder.Services.AddTransient<Application.Abstractions.IExternalWorkoutProvider>(sp =>
{
    var httpImpl = sp.GetRequiredService<Infrastructure.External.HttpExternalWorkoutProvider>();
    var cache = sp.GetRequiredService<Microsoft.Extensions.Caching.Memory.IMemoryCache>();
    var logger = sp.GetRequiredService<ILogger<Infrastructure.External.CachingExternalWorkoutProvider>>();
    var cacheOpts = sp.GetRequiredService<Microsoft.Extensions.Options.IOptions<Infrastructure.External.ExternalWorkoutCacheOptions>>().Value;
    return new Infrastructure.External.CachingExternalWorkoutProvider(httpImpl, cache, logger, cacheOpts);
});

builder.Services.AddDataProtection()
    .PersistKeysToFileSystem(new System.IO.DirectoryInfo(System.IO.Path.Combine(builder.Environment.ContentRootPath, "DataProtectionKeys")))
    .SetApplicationName("GymTrainer");

builder.Services.AddSession(options =>
{
    options.Cookie.Name = ".GymTrainer.Session";
    options.Cookie.HttpOnly = true;
    options.IdleTimeout = System.TimeSpan.FromHours(8);
});

var app = builder.Build();

app.UseStaticFiles();
app.UseGlobalExceptionHandling();
app.UseSession();
app.MapControllers();
app.MapRazorPages();

using (var scope = app.Services.CreateScope())
{
    var ctx = scope.ServiceProvider.GetRequiredService<GymDbContext>();
    ctx.Database.Migrate();
    var seeder = new Infrastructure.Persistence.Seeding.DatabaseSeeder(ctx);
    seeder.Seed();
}

app.Run();