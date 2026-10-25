# CLAUDE.md

This project follows a standard Clean Architecture + CQRS layout for ASP.NET Core Web APIs.
These rules apply to every change. When existing code disagrees with them, follow the rules
for new code and mention the discrepancy rather than silently copying it.

`<Solution>` below means the solution prefix (e.g. `ATTM.MeetingApp`), `<Entity>` a domain
entity in singular PascalCase (e.g. `Meeting`), and `<Entities>` its plural (e.g. `Meetings`).

## Commands

```bash
dotnet build                                   # build the whole solution
dotnet test                                    # run all tests
dotnet run --project <Solution>.API            # run the API (Swagger at /swagger)

# EF Core migrations (DbContext lives in Persistence, startup is the API)
dotnet ef migrations add <Name> --project <Solution>.Persistence --startup-project <Solution>.API
dotnet ef database update      --project <Solution>.Persistence --startup-project <Solution>.API
```

Run `dotnet build` and `dotnet test` before considering a change done.

## Solution layout

```
<Solution>.sln
<Solution>.Domain/          Entities and enums. No dependencies.
<Solution>.Application/     Use cases: commands, queries, handlers, validators, VMs, contracts.
<Solution>.Persistence/     EF Core DbContext, entity configurations, repositories, migrations.
<Solution>.API/             Controllers, middleware, Program.cs, configuration.
<Solution>.UnitTests/       Handler and validator tests against mocked contracts.
```

Additional infrastructure (email, file storage, external APIs) goes in `<Solution>.Infrastructure`,
structured like Persistence: implements interfaces from `Application/Contracts`, registers itself
via its own `Add...Services` extension.

## Dependency rule (never violate)

```
API ──► Application ──► Domain
 │          ▲
 └──► Persistence ──┘
```

- **Domain** references nothing — no EF, no MediatR, no serialization attributes.
- **Application** references only Domain. It defines interfaces (`Contracts/`) that outer layers implement.
  It never references EF Core, ASP.NET Core or Azure SDKs.
- **Persistence / Infrastructure** reference Application (to implement its contracts) and Domain.
- **API** references Application and Persistence/Infrastructure only to call their registration extensions.
  Controllers never touch a repository or DbContext directly.
- **UnitTests** reference Application and Domain, never Persistence or API.

If a change seems to require breaking this rule, stop and ask.

## Domain layer

- `Entities/<Entity>.cs` — plain classes with `Guid Id` as key.
- Enums live next to entities (`Entities/<Name>Enum.cs` or a descriptive name).
- No persistence, transport or framework concerns here. JSON enum handling is configured globally in the API.

## Application layer

### Feature slices (CQRS)

Every use case is its own folder. One class per file; file name = class name.

```
Features/<Entities>/
  Commands/
    Create<Entity>/  Create<Entity>Command.cs  Create<Entity>CommandHandler.cs  Create<Entity>CommandValidator.cs
    Update<Entity>/  Update<Entity>Command.cs  Update<Entity>CommandHandler.cs  Update<Entity>CommandValidator.cs
    Delete<Entity>/  Delete<Entity>Command.cs  Delete<Entity>CommandHandler.cs
  Queries/
    Get<Entity>/      Get<Entity>Query.cs      Get<Entity>QueryHandler.cs      <Entity>VM.cs
    Get<Entity>List/  Get<Entity>ListQuery.cs  Get<Entity>ListQueryHandler.cs  <Entity>ListVM.cs
```

- Namespace matches the folder: `<Solution>.Application.Features.<Entities>.Commands.Create<Entity>`.
- Commands/queries are immutable `record`s implementing MediatR `IRequest` / `IRequest<T>`.
- Create returns the new `Guid`. Update and Delete return nothing. Queries return a VM or `List<VM>`.
- Create commands do not carry an `Id`; the server generates it.
- Handlers are `IRequestHandler<TRequest, TResponse>`, depend only on `Application/Contracts`
  interfaces and `IMapper`, and pass `CancellationToken` through to every async call.
- Handlers never return Domain entities to the caller — always map to a VM.
- A VM belongs to the query that returns it. Don't share VMs across features; duplicate instead.
- List VMs contain the subset of fields a list needs, but always include `Id`.

### Validation

- One FluentValidation `AbstractValidator<TCommand>` per command that takes input.
- Validators are registered from the assembly and run by a MediatR `ValidationBehavior<TRequest, TResponse>`
  pipeline behavior (`Behaviors/ValidationBehavior.cs`). Handlers do **not** construct or call validators.
- The behavior throws `ValidationException` (from `Application/Exceptions`) with all error messages.
- Validators check input shape only (required, lengths, ranges, `IsInEnum()` for enums).
  Existence checks belong in the handler and throw `NotFoundException`.
- Limits shared with the database (e.g. max lengths) are defined once as constants and used by both
  validators and EF configurations.

### Contracts

- `Contracts/IAsyncRepository<T>` — generic CRUD:
  `GetAsync(Guid id, CancellationToken)`, `ListAllAsync(CancellationToken)`, `AddAsync`, `UpdateAsync`, `DeleteAsync`.
- `Contracts/I<Entity>Repository : IAsyncRepository<<Entity>>` — one per aggregate, holds entity-specific queries.
- Handlers depend on `I<Entity>Repository`, never on `IAsyncRepository<<Entity>>` directly.
- Any other outside dependency (clock, current user, email…) gets an interface here.

### Exceptions

`Application/Exceptions/` holds the exceptions the API translates to HTTP status codes:
`NotFoundException` (404), `ValidationException` (400). Add new ones here, and add their mapping to the
API exception middleware in the same change.

### Mapping

- AutoMapper profiles live in Application (`MappingProfiles.cs`, or one `Profile` per feature area once it grows).
- Every new command/VM gets its `CreateMap` in the same change.

### Registration

`ApplicationServiceRegistration.AddApplicationServices(this IServiceCollection)` registers AutoMapper,
MediatR, validators and pipeline behaviors, scanning `typeof(ApplicationServiceRegistration).Assembly`
(never `AppDomain.CurrentDomain.GetAssemblies()`).

## Persistence layer

- `<Solution>DbContext` (or `<Product>DbContext`) exposes one `DbSet<T>` per aggregate and calls
  `modelBuilder.ApplyConfigurationsFromAssembly(...)`. No inline entity configuration in `OnModelCreating`.
- `Configurations/<Entity>Configuration.cs : IEntityTypeConfiguration<<Entity>>` — table, key, constraints, seed data.
- Seed data uses fixed, hard-coded GUIDs (never `Guid.NewGuid()`).
- `Repositories/BaseRepository<T> : IAsyncRepository<T>` implements the generic CRUD.
  `Repositories/<Entity>Repository : BaseRepository<<Entity>>, I<Entity>Repository` adds specific queries.
- Read-only queries use `AsNoTracking()`; push filtering/sorting/paging into the database query, not into LINQ-to-objects.
- `Migrations/` is generated by `dotnet ef` — never hand-edit, never suppress `PendingModelChangesWarning`.
- `PersistenceServiceRegistration.AddPersistenceServices(this IServiceCollection, IConfiguration)` registers
  the DbContext and repositories. It reads the connection string with `GetConnectionString("Database")`;
  it does not know where configuration comes from.
- The database provider is chosen once per project. Only the Persistence `.csproj`, the `Use<Provider>(...)` call
  in `PersistenceServiceRegistration` and the generated `Migrations/` know which provider it is. Switching provider
  means changing those and regenerating migrations — nothing else in the solution changes.

## API layer

### Controllers

- `Controllers/<Entity>Controller.cs`, `[ApiController]`, `[Route("api/[controller]")]`, inherits `ControllerBase`.
- The only injected dependency is `IMediator`. Each action builds a command/query, sends it, and returns a result.
  No business logic, no try/catch, no mapping.
- REST conventions:

  | Action | Verb / route         | Returns                                  |
  |--------|----------------------|------------------------------------------|
  | List   | `GET /`              | `200` `List<<Entity>ListVM>`             |
  | Get    | `GET /{id}`          | `200` `<Entity>VM` / `404`               |
  | Create | `POST /`             | `201` via `CreatedAtRoute("Get<Entity>")`|
  | Update | `PUT /{id}`          | `204` / `400` / `404`                    |
  | Delete | `DELETE /{id}`       | `204` / `404`                            |

- Every action declares `[ProducesResponseType]` for each status it can return, and has a `Name`.
- Accept `CancellationToken` in actions and pass it to `_mediator.Send`.

### Errors

A single exception middleware (`Middleware/ExceptionHandlerMiddleware.cs`, registered via
`UseCustomExceptionHandler()` as the first middleware) maps Application exceptions to status codes
and returns a JSON body (prefer `ProblemDetails`). Unknown exceptions → `500` with a generic message
and the exception logged — never the raw exception message.

### Program.cs

Keep it a short composition root:
1. `AddApplicationServices()`, `AddPersistenceServices(configuration)`, other `Add...Services`.
2. Controllers with `JsonStringEnumConverter`, Swagger.
3. Middleware pipeline and `MapControllers()`.

### Configuration and secrets

- The app is hosting-agnostic: it uses only the default configuration sources (appsettings, user-secrets in
  Development, environment variables, command line). No code references a specific secret store or cloud SDK.
- The deployment environment supplies settings and secrets through those sources, e.g. environment variable
  `ConnectionStrings__Database` (from App Service settings, Key Vault references, Kubernetes secrets, …).
- Secrets never go in committed `appsettings*.json`. Locally use user-secrets or the git-ignored
  `appsettings.Development.json`.

## Tests

- xUnit + Moq. Folder structure mirrors Application: `<Entities>/Commands/`, `<Entities>/Queries/`, `<Entities>/Validators/`.
- `Mocks/RepositoryMocks.cs` provides in-memory `Mock<I<Entity>Repository>` builders.
- Use the real AutoMapper profile (`new MapperConfiguration(c => c.AddProfile<MappingProfiles>())`), not a mocked `IMapper`.
- Each new handler gets at least a happy-path test and a not-found/invalid test; each validator gets tests per rule.
- Test names: `<Handler>_<Scenario>_<Expected>`.

## Skills

Use these skills (in `.claude/skills/` or `~/.claude/skills/`) instead of working from memory:

- `add-entity` — new entity with full CRUD through all layers, from templates.
- `add-use-case` — a single command or query on an existing entity.
- `add-infrastructure-service` — an external dependency behind an Application contract.
- `architecture-check` — audit the solution against these rules (`check.sh` + review list).

## Adding a new entity — checklist

1. `Domain/Entities/<Entity>.cs` (+ enums).
2. `Application/Contracts/I<Entity>Repository.cs`.
3. `Application/Features/<Entities>/...` — commands, queries, handlers, validators, VMs.
4. Mappings in the AutoMapper profile.
5. `Persistence/Configurations/<Entity>Configuration.cs`, `DbSet` on the context, `Repositories/<Entity>Repository.cs`,
   registration in `AddPersistenceServices`.
6. `dotnet ef migrations add Add<Entity>`.
7. `API/Controllers/<Entity>Controller.cs`.
8. Unit tests for handlers and validators.
9. `dotnet build && dotnet test`.

## Style

- File-scoped namespaces, nullable reference types and implicit usings enabled; remove unused `using`s.
- Private fields `_camelCase`, `readonly` where possible; prefer primary constructors for DI-only classes.
- `async` all the way; every async method that can take a `CancellationToken` takes one.
- No commented-out code and no empty `<Folder Include>` entries in `.csproj` files.
- One public type per file.
