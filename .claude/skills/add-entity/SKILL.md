---
name: add-entity
description: Add a new domain entity with full CRUD through every layer of a Clean Architecture + CQRS ASP.NET Core solution (Domain, Application commands/queries/validators/VMs, Persistence configuration/repository/migration, API controller, unit tests). Use when the user asks to add an entity, aggregate, resource, table, or "CRUD for X" in a solution laid out as <Solution>.Domain / .Application / .Persistence / .API / .UnitTests.
---

# Add an entity

Adds `<Entity>` end to end, following the project's CLAUDE.md. Templates live in `templates/` next to this file.

## 1. Gather inputs

Determine, asking the user only for what can't be inferred:

| Placeholder      | Example          | Source                                                        |
|------------------|------------------|---------------------------------------------------------------|
| `{{Solution}}`   | `ATTM.MeetingApp`| Prefix of the `*.Domain.csproj` file                          |
| `{{Entity}}`     | `Room`           | User (singular PascalCase)                                    |
| `{{Entities}}`   | `Rooms`          | Plural of Entity                                              |
| `{{entity}}`     | `room`           | camelCase Entity                                              |
| `{{entities}}`   | `rooms`          | camelCase Entities                                            |
| `{{DbContext}}`  | `MeetingDbContext`| The class deriving from `DbContext` in Persistence           |
| Properties       | `Name: string(100), Capacity: int > 0, Kind: RoomKind enum` | User |
| Operations       | all of Create/Update/Delete/Get/GetList (default) | User      |

For each property decide: C# type, required?, string max length, numeric range, enum. Max lengths become
`public const int <Prop>MaxLength` on the entity so validators and EF config share them.

## 2. Check the foundation

The entity templates assume these exist. For each one that is missing, create it from `templates/foundation/`
(replace placeholders, drop the `.tmpl`). If an existing file differs from the template in a way the entity
templates rely on (e.g. repository methods without `CancellationToken`, no `ValidationBehavior`), tell the user
and ask before changing shared code.

| File                                                        | Template                               |
|-------------------------------------------------------------|----------------------------------------|
| `Application/Contracts/IAsyncRepository.cs`                 | `IAsyncRepository.cs.tmpl`             |
| `Application/Exceptions/NotFoundException.cs`               | `NotFoundException.cs.tmpl`            |
| `Application/Exceptions/ValidationException.cs`             | `ValidationException.cs.tmpl`          |
| `Application/Behaviors/ValidationBehavior.cs`               | `ValidationBehavior.cs.tmpl`           |
| `Application/MappingProfiles.cs`                            | `MappingProfiles.cs.tmpl`              |
| `Application/ApplicationServiceRegistration.cs`             | `ApplicationServiceRegistration.cs.tmpl` |
| `Persistence/<DbContext>.cs`                                | `AppDbContext.cs.tmpl`                 |
| `Persistence/Repositories/BaseRepository.cs`                | `BaseRepository.cs.tmpl`               |
| `Persistence/PersistenceServiceRegistration.cs`             | `PersistenceServiceRegistration.cs.tmpl` |
| `API/Middleware/ExceptionHandlerMiddleware.cs`, `MiddlewareExtensions.cs` | same names               |

Packages the foundation needs:

| Project     | Packages                                                                                   |
|-------------|--------------------------------------------------------------------------------------------|
| Application | `MediatR`, `AutoMapper`, `FluentValidation`, `FluentValidation.DependencyInjectionExtensions` |
| Persistence | the EF Core provider (e.g. `Npgsql.EntityFrameworkCore.PostgreSQL`), `Microsoft.EntityFrameworkCore.Design` |
| API         | `Swashbuckle.AspNetCore`; with Key Vault: `Azure.Extensions.AspNetCore.Configuration.Secrets`, `Azure.Identity` |
| UnitTests   | `xunit`, `xunit.runner.visualstudio`, `Microsoft.NET.Test.Sdk`, `Moq`                       |

The foundation templates target MediatR 12 (`next()` without arguments) and AutoMapper 13. On MediatR 13+, call
`next(cancellationToken)` in `ValidationBehavior`.

## 3. Generate files

Copy each template, replace placeholders, and replace every `// PROPERTIES` block with the real properties
(the `Name` property in the templates is only an example — remove it if the entity has no `Name`).
Skip files for operations the user doesn't want.

| Template (`templates/entity/`)      | Destination                                                                          |
|-------------------------------------|--------------------------------------------------------------------------------------|
| `Domain.Entity.cs.tmpl`             | `<Solution>.Domain/Entities/<Entity>.cs`                                             |
| `Application.IEntityRepository.cs.tmpl` | `<Solution>.Application/Contracts/I<Entity>Repository.cs`                        |
| `Create.Command.cs.tmpl`            | `<Solution>.Application/Features/<Entities>/Commands/Create<Entity>/Create<Entity>Command.cs` |
| `Create.CommandHandler.cs.tmpl`     | `.../Create<Entity>/Create<Entity>CommandHandler.cs`                                 |
| `Create.CommandValidator.cs.tmpl`   | `.../Create<Entity>/Create<Entity>CommandValidator.cs`                               |
| `Update.*.cs.tmpl`                  | `.../Commands/Update<Entity>/Update<Entity>{Command,CommandHandler,CommandValidator}.cs` |
| `Delete.*.cs.tmpl`                  | `.../Commands/Delete<Entity>/Delete<Entity>{Command,CommandHandler}.cs`              |
| `Get.*.cs.tmpl`                     | `.../Queries/Get<Entity>/{Get<Entity>Query,Get<Entity>QueryHandler,<Entity>VM}.cs`   |
| `GetList.*.cs.tmpl`                 | `.../Queries/Get<Entity>List/{Get<Entity>ListQuery,Get<Entity>ListQueryHandler,<Entity>ListVM}.cs` |
| `Persistence.Configuration.cs.tmpl` | `<Solution>.Persistence/Configurations/<Entity>Configuration.cs`                     |
| `Persistence.Repository.cs.tmpl`    | `<Solution>.Persistence/Repositories/<Entity>Repository.cs`                          |
| `API.Controller.cs.tmpl`            | `<Solution>.API/Controllers/<Entity>Controller.cs`                                   |
| `Tests.HandlerTests.cs.tmpl`        | `<Solution>.UnitTests/<Entities>/<Entity>HandlerTests.cs`                            |
| `Tests.ValidatorTests.cs.tmpl`      | `<Solution>.UnitTests/<Entities>/Create<Entity>CommandValidatorTests.cs`             |

## 4. Wire it up (edit existing files)

1. `MappingProfiles.cs` — append `Mapping.snippet.tmpl` inside the constructor, add the feature `using`s.
2. `<DbContext>.cs` — add `public DbSet<<Entity>> <Entities> => Set<<Entity>>();`.
3. `PersistenceServiceRegistration.cs` — add `services.AddScoped<I<Entity>Repository, <Entity>Repository>();`.
4. `UnitTests/Mocks/RepositoryMocks.cs` — add `Tests.RepositoryMock.snippet.tmpl` (create the class if missing:
   `public static class RepositoryMocks` with `using Moq;` and the Application/Domain usings).
5. Update the test templates' constructor calls and command arguments to the real properties.

## 5. Migration

If `dotnet ef` is available:
```bash
dotnet ef migrations add Add<Entity> --project <Solution>.Persistence --startup-project <Solution>.API
```
Otherwise tell the user to run it. Never hand-write migration files.

## 6. Verify

```bash
dotnet build && dotnet test
```
Then run the `architecture-check` skill if it is installed. Report the files created and edited, and anything
skipped (e.g. migration not generated).

## Rules that are easy to get wrong

- Create command has **no** `Id`. Update gets `Id` from the route via `command with { Id = id }`.
- Handlers never construct validators — `ValidationBehavior` runs them.
- Handlers depend on `I<Entity>Repository`, not `IAsyncRepository<<Entity>>`.
- Get/Update/Delete throw `NotFoundException` for an unknown id; the controller has no null checks.
- Enum rules use `.IsInEnum()`, not `.NotEmpty()` (the first enum value is `0`, which `NotEmpty` rejects).
- Seed data uses fixed GUIDs.
