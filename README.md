# Gym Trainer — backend (краткая справка)

Это .NET (C#) бэкенд для оптимизации тренировок. Проект организован по принципам Clean Architecture:

- `Domain/` — модели сущностей
- `Application/` — сервисы и бизнес-логика
- `Infrastructure/` — DbContext и миграции
- `Web/` — API (Controllers) и UI (Razor Pages)

Ключевые детали
--------------
- SQLite используется как локальная тестовая БД. Файл БД находится в `src/Web/gym.db` и приложение настроено на использование этого файла относительно `ContentRootPath`.
- PageModel-ы (Razor Pages) обращаются напрямую к сервисам через DI (AuthService, RecommendationService, TrainingOptimizerService, TrainingHistoryService). Внутренние HTTP-вызовы к собственному API были удалены.
- Пароли хранятся как SHA256-хеш: `Domain/UserAuth.HashPassword`. Не меняйте эту логику без миграций и тестов.

Запуск проекта локально
----------------------
1. Убедитесь, что установлен dotnet SDK (7/8+):
```powershell
dotnet --info
```
2. Соберите и запустите Web-проект (рекомендуется указывать путь к csproj):
```powershell
dotnet build "d:\gym trainer\src\Web\Web.csproj"
dotnet run --project "d:\gym trainer\src\Web\Web.csproj"
```
3. В логах старта Kestrel будет указан URL (обычно `http://localhost:5000`). Откройте браузер по этому адресу.

Миграции и управление схемой БД
-----------------------------
- Установите EF Core tools (однократно):
```powershell
dotnet tool install --global dotnet-ef
```
# Gym Trainer — краткая инструкция для разработчиков (на русском)

Это backend на .NET (C#) для генерации/оптимизации тренировочных сплитов. Код организован в стилe Clean Architecture:

- `Domain/` — модели сущностей
- `Application/` — сервисы и бизнес-логика
- `Infrastructure/` — EF Core DbContext и миграции
- `Web/` — API (Controllers) и UI (Razor Pages)

Ключевые моменты
---------------
- Локальная тестовая БД: `src/Web/gym.db` (SQLite). Приложение настраивает путь как Path.Combine(ContentRootPath, "gym.db").
- PageModel-ы используют DI (AuthService, TrainingOptimizerService, TrainingHistoryService, RecommendationService).
- Пароли хранятся как SHA256-хеш (см. `Domain/UserAuth.HashPassword`). Не меняйте это без явной договорённости и тестов.

Быстрый старт (PowerShell)
-------------------------
1) Перейдите в папку проекта и соберите решение:
```powershell
cd 'd:\gym trainer\src\Web'
dotnet build
```

2) Установите EF Core tools (если ещё не установлен):
```powershell
dotnet tool install --global dotnet-ef
```

3) Примените миграции и запустите приложение (рекомендуется явно указать startup-project для миграций):
```powershell
cd 'd:\gym trainer\src\Infrastructure'
dotnet ef database update --startup-project ../Web
cd 'd:\gym trainer\src\Web'
dotnet run --urls "http://localhost:5002"
```

4) Откройте браузер: http://localhost:5002

Миграции и управление схемой
---------------------------
- Чтобы создать миграцию (в `src/Infrastructure`):
```powershell
dotnet ef migrations add <MigrationName> --startup-project ../Web
```
- Чтобы применить миграции:
```powershell
dotnet ef database update --startup-project ../Web
```

Короткие пользовательские сценарии (где смотреть в UI/API)
---------------------------------------------------------
- Регистрация: страница `/Register` → вызывает `AuthService.Register` → POST `/api/auth/register`.
- Вход: страница `/Login` → `AuthService.Login` → POST `/api/auth/login`. При успешном входе в сессию записывается `UserId` и происходит Redirect на `/Profile`.
- Просмотр истории: `/History` (использует `TrainingHistoryService`).
- Получить оптимальный сплит: `/Optimize` (через `TrainingOptimizerService`).
- Фильтрация сплитов по сложности реализована: `TrainingSplit` имеет поле `Difficulty` (Easy/Medium/Hard). Страница `Workouts` поддерживает фильтр по query-string `?difficulty=easy|medium|hard`.

Рекомендации для разработчиков
-----------------------------
- Не меняйте названия сущностей/полей без миграций.
- Для автоматизированного тестирования используйте `WebApplicationFactory<TEntryPoint>` и временную SQLite DB.
- Для UI-изменений предпочитайте инъекцию сервисов напрямую в PageModel-ы (вместо внутренних HTTP-вызовов).

Что я сделал в кодовой базе
---------------------------
- Упростил PageModel-ы — они используют DI, а не HttpClient.
- Добавил сидер в `Program.cs`, который заполняет БД примерами упражнений и сплитов при пустой базе.
- Добавил поле `Difficulty` в `TrainingSplit` и миграцию `AddDifficultyToTrainingSplit`.
- Добавил live-поведение ползунка усталости, бейджи сложности в списке тренировок и базовые ARIA-атрибуты для форм.

Дальнейшие варианты (предложение)
--------------------------------
1) Добавить UI-контролы: селектор сложности на страницах `Workouts` и `Optimize` (я могу сделать это и собрать проект).
2) Расширить сидер — добавить больше упражнений/сплитов для демонстрации.
3) Написать интеграционный тест: регистрация → логин → optimize → log history.
4) Обновить стили/карточки UI.

Если хотите, начну с пункта 1 (добавить UI-контролы и связать их с текущей фильтрацией). Напишите номер варианта.
