---
name: add-infrastructure-service
description: Add an external dependency (email/SMTP, blob or file storage, an HTTP API client, message bus, cache, clock, current-user service, PDF generation, etc.) to a Clean Architecture ASP.NET Core solution — interface in Application/Contracts, implementation in an Infrastructure project, options bound from configuration, DI registration, and test mocks. Use whenever a handler needs something outside the database.
---

# Add an infrastructure service

Handlers depend on an interface they own; the outside world is implemented in `<Solution>.Infrastructure`.
The database is the only exception — it stays in `<Solution>.Persistence`.

## 1. Contract (Application)

`<Solution>.Application/Contracts/<Area>/I<Service>.cs` — express what the use case needs, in domain terms,
not the vendor's API:

```csharp
namespace <Solution>.Application.Contracts.<Area>;

public interface IEmailSender
{
    Task SendAsync(EmailMessage message, CancellationToken cancellationToken = default);
}
```

Request/response types the contract uses (`EmailMessage`) live next to it in Application (`Models/<Area>/`).
No vendor SDK types in signatures.

## 2. Infrastructure project (create once)

If `<Solution>.Infrastructure` doesn't exist:

```bash
dotnet new classlib -n <Solution>.Infrastructure -f <same TargetFramework as the other projects>
dotnet sln add <Solution>.Infrastructure
dotnet add <Solution>.Infrastructure reference <Solution>.Application
dotnet add <Solution>.API reference <Solution>.Infrastructure
dotnet add <Solution>.Infrastructure package Microsoft.Extensions.Options.ConfigurationExtensions
```

Enable `Nullable` and `ImplicitUsings`, delete `Class1.cs`, and create:

```csharp
namespace <Solution>.Infrastructure;

public static class InfrastructureServiceRegistration
{
    public static IServiceCollection AddInfrastructureServices(this IServiceCollection services, IConfiguration configuration)
    {
        return services;
    }
}
```

Call `builder.Services.AddInfrastructureServices(builder.Configuration);` in `Program.cs` next to the other `Add...Services`.

## 3. Implementation

```
<Solution>.Infrastructure/<Area>/
  <Vendor><Service>.cs          e.g. SendGridEmailSender : IEmailSender
  <Service>Settings.cs          options class: public const string SectionName = "<Area>";
```

- Settings bound with `services.Configure<<Service>Settings>(configuration.GetSection(<Service>Settings.SectionName));`
  and injected as `IOptions<<Service>Settings>`.
- HTTP clients: `services.AddHttpClient<I<Service>, <Impl>>(...)` (typed client), base address from settings.
- Register with the right lifetime (stateless/HTTP → transient or typed client; holds connections → singleton).
- Vendor SDK packages are referenced by Infrastructure only.
- Secrets: add the config keys to `appsettings.json` with empty values, document them, and store the real values in
  user-secrets locally and Key Vault in deployed environments (`<Area>--ApiKey` naming). Never commit secrets.

## 4. Use it

Inject `I<Service>` into the handler that needs it. Controllers never call infrastructure services directly.

## 5. Tests

Mock `I<Service>` with Moq in handler tests and `Verify` the call. Infrastructure implementations are not
unit-tested against the real vendor; if worth testing, use an integration test project.

## 6. Verify

`dotnet build && dotnet test`, then the `architecture-check` skill if installed.
