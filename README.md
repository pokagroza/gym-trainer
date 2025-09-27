# Gym Trainer — учебный проект СМОЛГУ (краткая справка)

- `Domain/` — модели сущностей
- `Application/` — сервисы и бизнес-логика
- `Infrastructure/` — DbContext и миграции
- `Web/` — API (Controllers) и UI (Razor Pages)
:

Ключевые моменты
---------------
- Локальная тестовая БД: `src/Web/gym.db` (SQLite). Приложение настраивает путь как Path.Combine(ContentRootPath, "gym.db").
- PageModel-ы используют DI (AuthService, TrainingOptimizerService, TrainingHistoryService, RecommendationService).
- Пароли хранятся как SHA256-хеш (см. `Domain/UserAuth.HashPassword`). Не меняйте это без явной договорённости и тестов.


Короткие пользовательские сценарии (где смотреть в UI/API)
---------------------------------------------------------
- Регистрация: страница `/Register` → вызывает `AuthService.Register` → POST `/api/auth/register`.
- Вход: страница `/Login` → `AuthService.Login` → POST `/api/auth/login`. При успешном входе в сессию записывается `UserId` и происходит Redirect на `/Profile`.
- Просмотр истории: `/History` (использует `TrainingHistoryService`).
- Получить оптимальный сплит: `/Optimize` (через `TrainingOptimizerService`).
- Фильтрация сплитов по сложности реализована: `TrainingSplit` имеет поле `Difficulty` (Easy/Medium/Hard). Страница `Workouts` поддерживает фильтр по query-string `?difficulty=easy|medium|hard`.


Расширения архитектуры (сентябрь 2025)
--------------------------------------
Добавлены следующие улучшения для более чистой архитектуры и расширения функциональности:

1. Слой Application теперь не зависит от Infrastructure.
	- Убрана ссылка `Application -> Infrastructure`.
	- Введён интерфейс `IAppDbContext` (в `Application.Abstractions`) с набором необходимых `DbSet<>` и доступом к `Database`.
	- `GymDbContext` реализует этот интерфейс. В DI регистрируется как `IAppDbContext`.

2. Репозиторный подход (пока частично):
	- Добавлен интерфейс `ICategoryRepository` в Application.
	- Реализация `CategoryRepository` в Infrastructure, использует `IAppDbContext`.

3. Категории упражнений:
	- Новая сущность `ExerciseCategory`.
	- В `Exercise` добавлены `ExerciseCategoryId` и навигация `Category`, также поле `Instructions` (для UI).
	- Seeder автоматически создаёт категории и мапит существующие упражнения по их `MuscleGroup`.
	- API контроллер: `GET/POST/PUT/DELETE /api/categories`.
	- Возвращаются DTO (`CategoryDto`), вход — `CategoryCreateUpdateDto`.

4. DTO и маппинг:
	- Файлы: `Web/Models/CategoryDtos.cs`, `Web/Models/CategoryMappers.cs`.
	- Контроллер больше не отдаёт EF сущности напрямую.

5. Миграции:
	- Добавлена миграция `AddExerciseCategory` (генерирует таблицу категорий + FK в `Exercises`).

Примеры API (Categories):
```
GET    /api/categories
GET    /api/categories/{id}
POST   /api/categories        { "name": "Грудь", "description": "Категория груди" }
PUT    /api/categories/{id}   { "name": "Грудь", "description": "Обновлено" }
DELETE /api/categories/{id}
```

Планы для дальнейшего улучшения (предложения):
- Вынести оставшиеся прямые EF обращения в репозитории (Training / History / Recommendation).
- Добавить юнит-тесты для всех репозиториев и сервисов (частично в процессе).
- Ввести отдельные запросы/команды (минимальный CQRS) для сложной логики оптимизации.
- Добавить middleware логирования/трэйсинга.
- Перейти на асинхронный EF (после выравнивания стиля во всех сервисах).


