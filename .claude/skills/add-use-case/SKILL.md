---
name: add-use-case
description: Add a single command or query (one CQRS use case) to an existing entity in a Clean Architecture + MediatR ASP.NET Core solution — e.g. "cancel a booking", "get rooms by location", "archive a meeting", "search customers". Creates the feature folder (request, handler, validator or VM), any repository method it needs, the mapping, the controller action and tests. Use for anything smaller than a whole new entity.
---

# Add a use case

One use case = one folder under `<Solution>.Application/Features/<Entities>/Commands|Queries/<UseCaseName>/`.

## 1. Classify

- **Command** — changes state. Name: `<Verb><Entity>` (`Cancel<Entity>`, `Archive<Entity>`, `Assign<Entity>Owner`).
  Returns `Guid` when it creates something, otherwise nothing (`IRequest`).
- **Query** — reads only. Name: `Get<Entity><Qualifier>` (`Get<Entity>sByLocation`, `Search<Entities>`).
  Returns its **own** VM (`<UseCaseName>VM` or `<Entity><Qualifier>VM`) — never reuse another feature's VM, never return an entity.

Confirm the name, inputs, and output with the user if not obvious.

## 2. Files

```
Features/<Entities>/Commands/<Name>/
  <Name>Command.cs            public record <Name>Command(Guid Id, ...) : IRequest;
  <Name>CommandHandler.cs
  <Name>CommandValidator.cs   only if the command has input beyond an Id
Features/<Entities>/Queries/<Name>/
  <Name>Query.cs              public record <Name>Query(...) : IRequest<List<<X>VM>>;
  <Name>QueryHandler.cs
  <X>VM.cs                    public record <X>VM { public Guid Id { get; init; } ... }
```

Namespace = folder path. Handler pattern (primary constructor, `I<Entity>Repository` + `IMapper`, pass `CancellationToken`):

```csharp
public class <Name>CommandHandler(I<Entity>Repository <entity>Repository)
    : IRequestHandler<<Name>Command>
{
    public async Task Handle(<Name>Command request, CancellationToken cancellationToken)
    {
        var <entity> = await <entity>Repository.GetAsync(request.Id, cancellationToken)
            ?? throw new NotFoundException(nameof(<Entity>), request.Id);

        // state change — prefer a method on the entity (<entity>.Cancel()) when the rule is domain logic
        await <entity>Repository.UpdateAsync(<entity>, cancellationToken);
    }
}
```

Business rules that a request can violate (e.g. "already cancelled") throw an exception from
`Application/Exceptions`. If no fitting exception exists, add one (e.g. `ConflictException`) **and** its status-code
mapping in `API/Middleware/ExceptionHandlerMiddleware.cs` in the same change.

## 3. Data access

If the query filters, sorts, pages, or projects, add a method to `I<Entity>Repository` and implement it in
`<Entity>Repository` with `AsNoTracking()` and the filter in the EF query. Don't load everything with
`ListAllAsync` and filter in memory. The method takes a `CancellationToken`.

## 4. Mapping

Add the `CreateMap` lines for the new VM / command to the AutoMapper profile.

## 5. Controller action

Add to `<Entity>Controller` (only `IMediator` injected, no logic):

| Use case              | Route                                    | Result                     |
|-----------------------|------------------------------------------|----------------------------|
| State-change command  | `POST {id:guid}/<verb>` (e.g. `/cancel`) | `204` / `400` / `404`      |
| Filtered query        | `GET` with `[FromQuery]` params, or `GET <qualifier>` | `200`          |

Every action: `Name = "<Name>"`, `[ProducesResponseType]` for each possible status, `CancellationToken` passed to `Send`.

## 6. Tests

In `<Solution>.UnitTests/<Entities>/`: a happy-path handler test, a not-found / rule-violation test, and one
test per validator rule. Extend `RepositoryMocks` with any new repository method.

## 7. Verify

`dotnet build && dotnet test`, then the `architecture-check` skill if installed.
