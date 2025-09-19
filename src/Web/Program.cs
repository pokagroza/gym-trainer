using Application.Services;
using Microsoft.AspNetCore.DataProtection;
using Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;
using System.IO;

var builder = WebApplication.CreateBuilder(args);

// Resolve the SQLite file path relative to the app content root so the bundled test DB is used reliably
var dbPath = Path.Combine(builder.Environment.ContentRootPath, "gym.db");
// Add services
builder.Services.AddDbContext<GymDbContext>(options =>
    options.UseSqlite($"Data Source={dbPath}"));
builder.Services.AddScoped<TrainingOptimizerService>();
builder.Services.AddScoped<TrainingHistoryService>();
builder.Services.AddScoped<AuthService>();
builder.Services.AddScoped<RecommendationService>();
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


// Configure middleware

app.UseStaticFiles();
app.UseSession();
app.MapControllers();
app.MapRazorPages();

// --- Seed DB on startup if empty (simple, idempotent) ---
using (var scope = app.Services.CreateScope())
{
    try
    {
        var ctx = scope.ServiceProvider.GetRequiredService<Infrastructure.Persistence.GymDbContext>();
        // Ensure core demo data exists; add missing entries by name so seeder is idempotent
        Action<string, string, string, string> addExerciseIfMissing = (name, muscle, equip, desc) =>
        {
            if (!ctx.Exercises.Any(e => e.Name == name))
            {
                ctx.Exercises.Add(new Domain.Entities.Exercise { Name = name, MuscleGroup = muscle, Equipment = equip, Description = desc });
            }
        };

        addExerciseIfMissing("Жим лежа", "Грудь", "Штанга", "Классическое базовое упражнение на грудные мышцы");
        addExerciseIfMissing("Приседания со штангой", "Ноги", "Штанга", "Базовое упражнение на ноги и корпус");
        addExerciseIfMissing("Тяга в наклоне", "Спина", "Штанга", "Укрепляет среднюю и нижнюю часть спины");
        addExerciseIfMissing("Подтягивания", "Спина", "Турник", "Функциональное упражнение с собственным весом");
        addExerciseIfMissing("Планка", "Кор", "Нет", "Статическое упражнение для удержания корпуса");
        addExerciseIfMissing("Бег на дорожке", "Кардио", "Беговая дорожка", "Кардио-нагрузки для выносливости");
        addExerciseIfMissing("Выпады", "Ноги", "Гантели", "Проработка квадрицепсов и ягодиц");
        addExerciseIfMissing("Жим гантелей сидя", "Плечи", "Гантели", "Изоляция дельтовидных мышц");
        addExerciseIfMissing("Становая тяга", "Спина/Ноги", "Штанга", "Мощное базовое упражнение");
        addExerciseIfMissing("Армейский жим", "Плечи", "Штанга", "Жим над головой для плеч и верхней части тела");
    addExerciseIfMissing("Ягодичный мост (Hip Thrust)", "Ягодицы", "Штанга", "Сильная нагрузка на ягодицы");
    addExerciseIfMissing("Бёрпи", "Кардио/Полное тело", "Нет", "Высокоинтенсивное комплексное упражнение");
    addExerciseIfMissing("Face Pull", "Плечи/Спина", "Трос", "Хорошо для задней дельты и здоровья плеч");
    addExerciseIfMissing("Румынская тяга", "Ноги/Ягодицы", "Штанга", "Изолированная работа задней поверхности бедра");
    addExerciseIfMissing("Фермерская прогулка", "Захват/Кор", "Гантели", "Функциональная нагрузка с удержанием");
    // Additional exercises for richer split variations
    addExerciseIfMissing("Отжимания на брусьях", "Грудь/Трицепс", "Брусья", "Отлично для силовой работы корпуса и верхней части");
    addExerciseIfMissing("Подъём штанги на бицепс", "Руки", "Штанга", "Изолированная проработка бицепса");
    addExerciseIfMissing("Тяга верхнего блока", "Спина", "Блок", "Контроль эксцентрической фазы и широчайшие");
    addExerciseIfMissing("Жим ногами", "Ноги", "Тренажёр", "Альтернатива приседаниям для объёма ног");
    addExerciseIfMissing("Кроссовер (Pec Deck)", "Грудь", "Трос", "Изоляция грудных мышц");
    addExerciseIfMissing("Русский твист", "Кор", "Медбол", "Динамическая проработка косых мышц");
    addExerciseIfMissing("Велотренажер", "Кардио", "Тренажёр", "Низкоинтенсивное кардио для восстановления");
    addExerciseIfMissing("Гиперэкстензия", "Спина/Кор", "Романейская скамья", "Проработка разгибателей корпуса");
    addExerciseIfMissing("Подъём гантелей в стороны", "Плечи", "Гантели", "Изолированная работа средней дельты");

        ctx.SaveChanges();

        // Helper to add split if missing
        Func<string, string, Domain.Entities.Difficulty, Domain.Entities.TrainingSplit> addSplitIfMissing = (name, desc, diff) =>
        {
            var exists = ctx.TrainingSplits.FirstOrDefault(s => s.Name == name);
            if (exists != null) return exists;
            var sp = new Domain.Entities.TrainingSplit { Name = name, Description = desc, Difficulty = diff };
            ctx.TrainingSplits.Add(sp);
            ctx.SaveChanges();
            return sp;
        };

        var splitEasy = addSplitIfMissing("Восстановление — Лёгкая тренировка (Full Body)", "Низкая интенсивность, фокус на подвижности и восстановлении", Domain.Entities.Difficulty.Easy);
        var splitCardio = addSplitIfMissing("Кардио HIIT", "Интервальная кардио-тренировка для жиросжигания и выносливости", Domain.Entities.Difficulty.Medium);
        var splitFull = addSplitIfMissing("Полное тело — Гипертрофия", "Сбалансированная hypertrophy тренировка для набора массы", Domain.Entities.Difficulty.Medium);
        var splitStrength = addSplitIfMissing("Силовая — Нагрузка", "Низкий объем, высокая интенсивность, силовые подходы", Domain.Entities.Difficulty.Hard);
        var splitPPL = addSplitIfMissing("Push/Pull/Legs — 3-дневный цикл", "Классический PPL цикл для прогресса по силе и массе", Domain.Entities.Difficulty.Medium);
    var splitMobility = addSplitIfMissing("Мобильность и стабильность", "Растяжка, контроль корпуса и суставная мобильность", Domain.Entities.Difficulty.Easy);
    var splitUpperLower = addSplitIfMissing("4-дневный Upper/Lower", "Чередование верх-низ для баланса объёма и восстановления", Domain.Entities.Difficulty.Medium);
    var splitUpperLowerStrength = addSplitIfMissing("Upper Strength / Lower Hypertrophy", "Сильностный верх и объёмный низ", Domain.Entities.Difficulty.Hard);
    var splitSpecialization = addSplitIfMissing("Специализация — Ноги/Ягодицы", "Фокус на нижней части тела с большим объёмом", Domain.Entities.Difficulty.Medium);
    var splitFullVaried = addSplitIfMissing("Full Body — Смешанный", "Смешанная тренировка с упором на разные качества", Domain.Entities.Difficulty.Medium);

        ctx.SaveChanges();

        // Helper to add split exercise if missing (by split and exercise names)
        Action<string, string, int, int> addSplitExerciseIfMissing = (splitName, exerciseName, sets, reps) =>
        {
            var split = ctx.TrainingSplits.FirstOrDefault(s => s.Name == splitName);
            var ex = ctx.Exercises.FirstOrDefault(e => e.Name == exerciseName);
            if (split == null || ex == null) return;
            if (!ctx.SplitExercises.Any(se => se.TrainingSplitId == split.Id && se.ExerciseId == ex.Id))
            {
                ctx.SplitExercises.Add(new Domain.Entities.SplitExercise { TrainingSplitId = split.Id, ExerciseId = ex.Id, Sets = sets, Reps = reps });
            }
        };

        // recovery full body
        addSplitExerciseIfMissing(splitEasy.Name, "Планка", 3, 40);
        addSplitExerciseIfMissing(splitEasy.Name, "Подтягивания", 3, 6);

        // HIIT/cardio
        addSplitExerciseIfMissing(splitCardio.Name, "Бёрпи", 6, 12);
        addSplitExerciseIfMissing(splitCardio.Name, "Бег на дорожке", 1, 15);

        // hypertrophy full
        addSplitExerciseIfMissing(splitFull.Name, "Жим лежа", 4, 8);
        addSplitExerciseIfMissing(splitFull.Name, "Тяга в наклоне", 4, 8);
        addSplitExerciseIfMissing(splitFull.Name, "Выпады", 3, 12);

        // strength
        addSplitExerciseIfMissing(splitStrength.Name, "Становая тяга", 5, 3);
        addSplitExerciseIfMissing(splitStrength.Name, "Приседания со штангой", 5, 5);

        // push/pull/legs sample (push day)
        addSplitExerciseIfMissing(splitPPL.Name, "Жим лежа", 4, 6);
        addSplitExerciseIfMissing(splitPPL.Name, "Армейский жим", 3, 8);


    // mobility
    addSplitExerciseIfMissing(splitMobility.Name, "Face Pull", 3, 15);
    addSplitExerciseIfMissing(splitMobility.Name, "Планка", 3, 60);

    // Full body varied additions
    addSplitExerciseIfMissing(splitFullVaried.Name, "Русский твист", 3, 20);
    addSplitExerciseIfMissing(splitFullVaried.Name, "Фермерская прогулка", 3, 40);

    // specialized legs / glutes
    addSplitExerciseIfMissing(splitSpecialization.Name, "Ягодичный мост (Hip Thrust)", 5, 8);
    addSplitExerciseIfMissing(splitSpecialization.Name, "Румынская тяга", 4, 8);
    addSplitExerciseIfMissing(splitSpecialization.Name, "Жим ногами", 4, 12);

    // upper/lower template
    addSplitExerciseIfMissing(splitUpperLower.Name, "Жим лежа", 4, 8);
    addSplitExerciseIfMissing(splitUpperLower.Name, "Тяга верхнего блока", 4, 10);
    addSplitExerciseIfMissing(splitUpperLower.Name, "Приседания со штангой", 4, 8);
    addSplitExerciseIfMissing(splitUpperLower.Name, "Ягодичный мост (Hip Thrust)", 4, 10);

    addSplitExerciseIfMissing(splitUpperLowerStrength.Name, "Жим лежа", 5, 4);
    addSplitExerciseIfMissing(splitUpperLowerStrength.Name, "Становая тяга", 5, 3);
    addSplitExerciseIfMissing(splitUpperLowerStrength.Name, "Подъём гантелей в стороны", 3, 12);

    // push/pull/legs extras
    addSplitExerciseIfMissing(splitPPL.Name, "Подъём штанги на бицепс", 3, 10);
    addSplitExerciseIfMissing(splitPPL.Name, "Жим гантелей сидя", 3, 10);

    // small extras
    addSplitExerciseIfMissing(splitFull.Name, "Жим гантелей сидя", 3, 10);
    addSplitExerciseIfMissing(splitFull.Name, "Ягодичный мост (Hip Thrust)", 4, 10);
    addSplitExerciseIfMissing(splitPPL.Name, "Тяга в наклоне", 4, 8);

    // cardio and conditioning options
    addSplitExerciseIfMissing(splitCardio.Name, "Велотренажер", 1, 20);
    addSplitExerciseIfMissing(splitCardio.Name, "Бёрпи", 8, 10);

    // core and accessory
    addSplitExerciseIfMissing(splitFull.Name, "Русский твист", 3, 18);
    addSplitExerciseIfMissing(splitFullVaried.Name, "Гиперэкстензия", 3, 12);

        ctx.SaveChanges();
    }
    catch { /* swallow seed errors for now */ }
}

app.Run();