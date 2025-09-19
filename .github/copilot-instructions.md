## Quick context

This repository is a small Clean-Architecture style .NET web backend for a gym training optimizer.
- Layers: `Domain/` (entities), `Application/` (services, business logic), `Infrastructure/` (EF Core DbContext + migrations), `Web/` (API controllers + Razor Pages).

## What to know up-front

- The app uses EF Core with SQLite. The active DB file is `src/Web/gym.db` by default and `GymDbContext` is configured in `src/Web/Program.cs`.
- Services are registered in `Program.cs` with scoped lifetime: `TrainingOptimizerService`, `TrainingHistoryService`, `AuthService`, `RecommendationService`.
- Controllers expose simple JSON APIs under `/api/*` (see `Web/*.cs` controllers). Many controllers interact directly with `GymDbContext` or the services in `Application/`.

## Common workflows (concrete commands)

- Run the web app (from workspace root):
  cd src/Web; dotnet run
- Install EF Core tools (one-time):
  dotnet tool install --global dotnet-ef
- Create a migration (from repo root):
  cd src/Infrastructure; dotnet ef migrations add InitialCreate --startup-project ../Web
- Apply migrations / update DB (from repo root):
  cd src/Infrastructure; dotnet ef database update --startup-project ../Web

Note: README.md contains similar instructions — prefer `--startup-project ../Web` when creating/applying migrations to ensure migrations use the `Web` startup.

## Project-specific patterns and gotchas

- The codebase uses simple, imperative EF Core queries inside service classes (examples: `Application/RecommendationService.cs`, `Application/TrainingOptimizerService.cs`). Prefer small, well-scoped edits and keep queries synchronous for now (SaveChanges() is used directly).
- `Domain/UserAuth.cs` implements a simple SHA256 password hash via `HashPassword`. Do not change authentication semantics without running integration tests — other code relies on this exact hashing implementation.
- Entity model conventions: navigation properties are nullable-initialized for EF (e.g., `User User { get; set; } = null!;`) and DbSets are defined in `Infrastructure/GymDbContext.cs`.
- The project targets .NET 7/8 (solution contains build outputs for both). Use dotnet SDK 7 or 8 to build/run.

## Where to make changes for common tasks

- Add business logic: `src/Application/*.cs` (services). Examples: `RecommendationService.cs`, `TrainingOptimizerService.cs`.
- Add or change DB schema: `src/Domain/*.cs` for entities, then add EF migration under `src/Infrastructure` using `--startup-project ../Web`.
- Add API endpoints: `src/Web/*Controller.cs` or Razor Pages under `src/Web/Pages/`.

## Tests and verification

- There are no unit tests in the repository. Before changing persistent behavior, run the app and exercise relevant endpoints (e.g., `GET /api/training/optimize/{userId}`, `POST /api/history`).

## Examples (small explicit snippets)

- To find the optimized split for user 1 the controller calls into service:
  - `GET /api/training/optimize/1` → `Web/TrainingController.cs` → `Application/TrainingOptimizerService.GetOptimizedSplit`.
- To register a user:
  - `POST /api/auth/register` with JSON `{ "username": "u", "password": "p", "name": "N", "age": 30, "height": 180, "weight": 80 }` → `Application/AuthService.Register` uses `UserAuth.HashPassword`.

## Safety and editing rules for AI agents

- Preserve existing database contracts: don't rename entity properties or DbSet names without adding a migration and updating references across layers.
- Keep `UserAuth.HashPassword` behavior intact unless you add a migration and update authentication logic across controllers.
- Avoid introducing asynchronous EF Core patterns unless you update all call sites consistently (controllers currently call sync methods).

## Helpful files to check while coding

- Startup and DI: `src/Web/Program.cs`
- DbContext and migrations: `src/Infrastructure/GymDbContext.cs`, `src/Infrastructure/Migrations/`
- Domain models: `src/Domain/*.cs`
- Business logic: `src/Application/*.cs`
- API surfaces: `src/Web/*.cs` and `src/Web/Pages/`

---
If any section is unclear or you want more examples (tests, debugging steps, or CI hooks), tell me which area to expand and I will iterate.
