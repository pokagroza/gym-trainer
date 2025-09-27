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

