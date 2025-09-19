using Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Persistence.Seeding;

public class DatabaseSeeder
{
    private readonly GymDbContext _ctx;

    public DatabaseSeeder(GymDbContext ctx)
    {
        _ctx = ctx;
    }

    public void Seed()
    {
        _ctx.Database.Migrate();
        AddExercises();
        AddSplits();
        _ctx.SaveChanges();
    }

    private void AddExercises()
    {
        void Add(string name, string muscle, string equip, string desc)
        {
            if (!_ctx.Exercises.Any(e => e.Name == name))
                _ctx.Exercises.Add(new Exercise { Name = name, MuscleGroup = muscle, Equipment = equip, Description = desc });
        }
        // Core set (mirrors previous seeding logic)
        Add("Жим лежа", "Грудь", "Штанга", "Классическое базовое упражнение на грудные мышцы");
        Add("Приседания со штангой", "Ноги", "Штанга", "Базовое упражнение на ноги и корпус");
        Add("Тяга в наклоне", "Спина", "Штанга", "Укрепляет среднюю и нижнюю часть спины");
        Add("Подтягивания", "Спина", "Турник", "Функциональное упражнение с собственным весом");
        Add("Планка", "Кор", "Нет", "Статическое упражнение для удержания корпуса");
        Add("Бег на дорожке", "Кардио", "Беговая дорожка", "Кардио-нагрузки для выносливости");
        Add("Выпады", "Ноги", "Гантели", "Проработка квадрицепсов и ягодиц");
        Add("Жим гантелей сидя", "Плечи", "Гантели", "Изоляция дельтовидных мышц");
        Add("Становая тяга", "Спина/Ноги", "Штанга", "Мощное базовое упражнение");
        Add("Армейский жим", "Плечи", "Штанга", "Жим над головой для плеч и верхней части тела");
        Add("Ягодичный мост (Hip Thrust)", "Ягодицы", "Штанга", "Сильная нагрузка на ягодицы");
        Add("Бёрпи", "Кардио/Полное тело", "Нет", "Высокоинтенсивное комплексное упражнение");
        Add("Face Pull", "Плечи/Спина", "Трос", "Хорошо для задней дельты и здоровья плеч");
        Add("Румынская тяга", "Ноги/Ягодицы", "Штанга", "Изолированная работа задней поверхности бедра");
        Add("Фермерская прогулка", "Захват/Кор", "Гантели", "Функциональная нагрузка с удержанием");
        Add("Отжимания на брусьях", "Грудь/Трицепс", "Брусья", "Отлично для силовой работы корпуса и верхней части");
        Add("Подъём штанги на бицепс", "Руки", "Штанга", "Изолированная проработка бицепса");
        Add("Тяга верхнего блока", "Спина", "Блок", "Контроль эксцентрической фазы и широчайшие");
        Add("Жим ногами", "Ноги", "Тренажёр", "Альтернатива приседаниям для объёма ног");
        Add("Кроссовер (Pec Deck)", "Грудь", "Трос", "Изоляция грудных мышц");
        Add("Русский твист", "Кор", "Медбол", "Динамическая проработка косых мышц");
        Add("Велотренажер", "Кардио", "Тренажёр", "Низкоинтенсивное кардио для восстановления");
        Add("Гиперэкстензия", "Спина/Кор", "Романейская скамья", "Проработка разгибателей корпуса");
        Add("Подъём гантелей в стороны", "Плечи", "Гантели", "Изолированная работа средней дельты");
    }

    private void AddSplits()
    {
        TrainingSplit EnsureSplit(string name, string desc, Difficulty diff)
        {
            var existing = _ctx.TrainingSplits.FirstOrDefault(s => s.Name == name);
            if (existing != null) return existing;
            var split = new TrainingSplit { Name = name, Description = desc, Difficulty = diff };
            _ctx.TrainingSplits.Add(split);
            _ctx.SaveChanges();
            return split;
        }

        var splitEasy = EnsureSplit("Восстановление — Лёгкая тренировка (Full Body)", "Низкая интенсивность, фокус на подвижности и восстановлении", Difficulty.Easy);
        var splitCardio = EnsureSplit("Кардио HIIT", "Интервальная кардио-тренировка для жиросжигания и выносливости", Difficulty.Medium);
        var splitFull = EnsureSplit("Полное тело — Гипертрофия", "Сбалансированная hypertrophy тренировка для набора массы", Difficulty.Medium);
        var splitStrength = EnsureSplit("Силовая — Нагрузка", "Низкий объем, высокая интенсивность, силовые подходы", Difficulty.Hard);
        var splitPPL = EnsureSplit("Push/Pull/Legs — 3-дневный цикл", "Классический PPL цикл для прогресса по силе и массе", Difficulty.Medium);
        var splitMobility = EnsureSplit("Мобильность и стабильность", "Растяжка, контроль корпуса и суставная мобильность", Difficulty.Easy);
        var splitUpperLower = EnsureSplit("4-дневный Upper/Lower", "Чередование верх-низ для баланса объёма и восстановления", Difficulty.Medium);
        var splitUpperLowerStrength = EnsureSplit("Upper Strength / Lower Hypertrophy", "Сильностный верх и объёмный низ", Difficulty.Hard);
        var splitSpecialization = EnsureSplit("Специализация — Ноги/Ягодицы", "Фокус на нижней части тела с большим объёмом", Difficulty.Medium);
        var splitFullVaried = EnsureSplit("Full Body — Смешанный", "Смешанная тренировка с упором на разные качества", Difficulty.Medium);

        void AddSplitExercise(TrainingSplit split, string exerciseName, int sets, int reps)
        {
            var ex = _ctx.Exercises.FirstOrDefault(e => e.Name == exerciseName);
            if (ex == null) return;
            if (!_ctx.SplitExercises.Any(se => se.TrainingSplitId == split.Id && se.ExerciseId == ex.Id))
                _ctx.SplitExercises.Add(new SplitExercise { TrainingSplitId = split.Id, ExerciseId = ex.Id, Sets = sets, Reps = reps });
        }

        // Sample assignments (same as previous Program.cs logic condensed)
        AddSplitExercise(splitEasy, "Планка", 3, 40);
        AddSplitExercise(splitEasy, "Подтягивания", 3, 6);
        AddSplitExercise(splitCardio, "Бёрпи", 6, 12);
        AddSplitExercise(splitCardio, "Бег на дорожке", 1, 15);
        AddSplitExercise(splitFull, "Жим лежа", 4, 8);
        AddSplitExercise(splitFull, "Тяга в наклоне", 4, 8);
        AddSplitExercise(splitFull, "Выпады", 3, 12);
        AddSplitExercise(splitStrength, "Становая тяга", 5, 3);
        AddSplitExercise(splitStrength, "Приседания со штангой", 5, 5);
        AddSplitExercise(splitPPL, "Жим лежа", 4, 6);
        AddSplitExercise(splitPPL, "Армейский жим", 3, 8);
        AddSplitExercise(splitMobility, "Face Pull", 3, 15);
        AddSplitExercise(splitMobility, "Планка", 3, 60);
        AddSplitExercise(splitFullVaried, "Русский твист", 3, 20);
        AddSplitExercise(splitFullVaried, "Фермерская прогулка", 3, 40);
        AddSplitExercise(splitSpecialization, "Ягодичный мост (Hip Thrust)", 5, 8);
        AddSplitExercise(splitSpecialization, "Румынская тяга", 4, 8);
        AddSplitExercise(splitSpecialization, "Жим ногами", 4, 12);
        AddSplitExercise(splitUpperLower, "Жим лежа", 4, 8);
        AddSplitExercise(splitUpperLower, "Тяга верхнего блока", 4, 10);
        AddSplitExercise(splitUpperLower, "Приседания со штангой", 4, 8);
        AddSplitExercise(splitUpperLower, "Ягодичный мост (Hip Thrust)", 4, 10);
        AddSplitExercise(splitUpperLowerStrength, "Жим лежа", 5, 4);
        AddSplitExercise(splitUpperLowerStrength, "Становая тяга", 5, 3);
        AddSplitExercise(splitUpperLowerStrength, "Подъём гантелей в стороны", 3, 12);
        AddSplitExercise(splitPPL, "Подъём штанги на бицепс", 3, 10);
        AddSplitExercise(splitPPL, "Жим гантелей сидя", 3, 10);
        AddSplitExercise(splitFull, "Жим гантелей сидя", 3, 10);
        AddSplitExercise(splitFull, "Ягодичный мост (Hip Thrust)", 4, 10);
        AddSplitExercise(splitPPL, "Тяга в наклоне", 4, 8);
        AddSplitExercise(splitCardio, "Велотренажер", 1, 20);
        AddSplitExercise(splitCardio, "Бёрпи", 8, 10);
        AddSplitExercise(splitFull, "Русский твист", 3, 18);
        AddSplitExercise(splitFullVaried, "Гиперэкстензия", 3, 12);
    }
}
