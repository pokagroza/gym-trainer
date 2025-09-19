using Application.Services;
using Microsoft.AspNetCore.DataProtection;
using Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;
using System.IO;
using Microsoft.Extensions.Hosting;

var builder = WebApplication.CreateBuilder(args);

// Resolve the SQLite file path relative to the app content root so the bundled test DB is used reliably
var dbPath = Path.Combine(builder.Environment.ContentRootPath, "gym.db");
// Add services
builder.Services.AddDbContext<GymDbContext>(options =>
    options.UseSqlite($"Data Source={dbPath}"));
builder.Services.AddScoped<ITrainingOptimizerService, TrainingOptimizerService>();
builder.Services.AddScoped<ITrainingHistoryService, TrainingHistoryService>();
builder.Services.AddScoped<IAuthService, AuthService>();
builder.Services.AddScoped<IRecommendationService, RecommendationService>();
builder.Services.AddControllers();
builder.Services.AddRazorPages();
// Persist data-protection keys to a local folder so session cookies can be unprotected
// across restarts and different process instances during development.
builder.Services.AddDataProtection()
    .PersistKeysToFileSystem(new System.IO.DirectoryInfo(System.IO.Path.Combine(builder.Environment.ContentRootPath, "DataProtectionKeys")))
    .SetApplicationName("GymTrainer");

// Configure session cookie options explicitly
builder.Services.AddSession(options =>
{
    options.Cookie.Name = ".GymTrainer.Session";
    options.Cookie.HttpOnly = true;
    options.IdleTimeout = System.TimeSpan.FromHours(8);
});

var app = builder.Build();

app.UseStaticFiles();
app.UseSession();
app.MapControllers();
app.MapRazorPages();

// Migration + seeding (scoped, idempotent)
using (var scope = app.Services.CreateScope())
{
    var ctx = scope.ServiceProvider.GetRequiredService<GymDbContext>();
    ctx.Database.Migrate();
    var seeder = new Infrastructure.Persistence.Seeding.DatabaseSeeder(ctx);
    seeder.Seed();
}

app.Run();