---
name: architecture-check
description: Audit a Clean Architecture + CQRS ASP.NET Core solution against its CLAUDE.md conventions — layer dependency rule, feature-slice layout, naming/namespaces, validation pipeline, repository contracts, EF configuration, thin controllers. Use when the user asks to review/check/audit the architecture, before finishing changes that add entities or use cases, or when asked "does this follow our conventions?".
---

# Architecture check

## 1. Run the mechanical checks

```bash
bash <path-to-this-skill>/check.sh <solution-root>
```

It finds the `*.Domain`, `*.Application`, `*.Persistence`, `*.API` and `*.UnitTests` projects and prints
PASS/FAIL per rule with `file:line` for every violation. Exit code 1 means at least one failure.

| Id  | Rule |
|-----|------|
| D1–D8 | Dependency rule: project references, forbidden packages/usings per layer, no framework attributes in Domain |
| A1  | Handlers don't construct validators |
| A2  | Handlers use `I<Entity>Repository`, not `IAsyncRepository<T>` |
| A3  | Assembly scanning via `typeof(...).Assembly` |
| A4  | A MediatR pipeline behavior exists |
| A5  | Contract async methods take `CancellationToken` |
| A6–A9 | Feature folder layout, names match folders, namespaces match folders, file name = type name |
| P1–P5 | No inline `OnModelCreating` config, no `Guid.NewGuid()` seeds, migration warning not suppressed, no secret fetching in Persistence, DB provider confined to Persistence |
| C1–C4 | `ControllerBase`, no repositories/mapper in controllers, no try/catch, 500 mapping present |
| H1  | No empty `<Folder Include>` entries |

The checks are grep-based: treat a FAIL as "look here", confirm it by reading the code, and ignore lines that are
clearly false positives (mention them in the report).

## 2. Review what a script can't

Read the code for these, citing `file:line`:

- **Thin controllers** — each action only builds a request and calls `mediator.Send`; status codes match the REST table in CLAUDE.md (`201 CreatedAtRoute`, `204`, `PUT /{id}`).
- **VMs** — queries return their own VM, never an entity; list VMs include `Id`; no VM shared between features.
- **Commands** — immutable records; Create has no `Id`; Update takes `Id` from the route.
- **Validation** — rules only check input shape; enums use `IsInEnum()`; limits come from shared constants that EF config also uses; Create and Update rules agree.
- **Handlers** — throw `NotFoundException` for unknown ids; pass `CancellationToken` everywhere; no filtering/sorting in memory after `ListAllAsync`.
- **Error mapping** — every exception in `Application/Exceptions` is mapped in the middleware; unknown exceptions give 500 without leaking the message.
- **Configuration** — Program.cs is a short composition root using only default configuration sources; no code references a specific secret store or cloud SDK; the app starts locally without cloud access.
- **Tests** — each handler has happy-path and not-found/invalid tests; each validator rule is tested; tests use the real AutoMapper profile.

## 3. Report

Group findings by layer, most structural first (dependency-rule violations before naming). For each: what's wrong,
`file:line`, and the fix. Finish with the script's pass/fail totals. Don't change code unless the user asks.
