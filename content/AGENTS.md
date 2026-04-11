# AGENTS.md — Clean.Architecture.Template

> This file is the single source of truth for AI agents working in this repository.
> Every rule and pattern below is derived from the actual codebase. Follow them exactly.

---

## Project Identity

| Property | Value |
|---|---|
| Solution file | `Clean.Architecture.Template.slnx` |
| Target framework | `net10.0` |
| Namespace convention | `Clean.Architecture.Template.{ProjectName}` (set in `Directory.Build.Props`) |
| Run command | `dotnet watch --project ./src/Api/Api.csproj` (or `./start.ps1`) |
| .env file | Root of solution (`../../.env` relative to the Api project) |
| .env.example | Root of solution (copy to `.env` and adjust) |
| global.json | Root of solution (SDK `10.0.0`, `rollForward: latestMinor`) |
| CI pipeline | `.github/workflows/ci.yml` (build + test on push/PR to main) |

---

## Hard Rules — Always Follow

1. **No exceptions for business logic.** Use `Result` and `Result<T>` (defined in `src/SharedKernel/Result.cs`) for all operation outcomes. Only throw for truly unrecoverable infrastructure failures.
2. **Use Mediator, NOT MediatR.** This project uses the source-generated [Mediator](https://github.com/martinothamar/Mediator) library (v3.0.2), not the reflection-based MediatR. All handler interfaces come from the `Mediator` namespace.
3. **Handlers return `ValueTask`, not `Task`.** All command/query handlers must return `ValueTask<TResponse>`.
4. **Commands implement `ICommand<Result<T>>`.** All command responses are wrapped in `Result` or `Result<T>`.
5. **Domain events inherit `INotification`.** Publish domain events for CRUD operations using Mediator's notification system.
6. **Domain error classes inherit `BaseErrors<T>`.** This provides standardized `NotFound`, `AlreadyExists`, `Failure`, `Conflict`, and `Problem` error factories. See `src/SharedKernel/BaseErrors.cs`.
7. **Entities implement `IIdentifiable<T>`.** All domain entities must implement this interface for identity management. See `src/SharedKernel/IIdentifiable.cs`.
8. **Use [Facet](https://github.com/Tim-Maes/Facet) for object mapping** when mapping between layers. See [Facet Documentation](https://github.com/Tim-Maes/Facet/wiki) for attribute-based mapping configuration.
9. **Use Serilog for all logging.** Serilog is configured in `src/Api/Program.cs` with structured logging, context enrichment, and console + Seq sinks.
10. **Nullable reference types are enabled** across all projects (`<Nullable>enable</Nullable>`).
11. **Do NOT use FluentValidation.** This project does not use FluentValidation or any similar validation library. Validation logic belongs in command/query handlers using the `Result`/`Error` pattern from SharedKernel.

---

## Dependency Graph (Allowed References)

```
Api → Application → Domain → SharedKernel
```

- **Api** may reference **Application** only.
- **Application** may reference **Domain** only.
- **Domain** may reference **SharedKernel** only.
- **SharedKernel** has no project references.
- **Never** reference a higher layer from a lower layer.

---

## Solution Structure

```
/Api           → Presentation layer (1 project: Api.csproj)
/Core          → Domain + Application + SharedKernel + Infrastructure (4 projects)
/Tests         → Application.Tests (xUnit + NSubstitute)
/Solution Items → .editorconfig, .gitignore, dotnet-tools.json, Nuget.Config, start.ps1
```

---

## Layer: SharedKernel

| Property | Value |
|---|---|
| Path | `src/SharedKernel/` |
| Namespace | `Clean.Architecture.Template.SharedKernel` |
| References | None |

### Key Types

| Type | File | Purpose |
|---|---|---|
| `Result` / `Result<T>` | `src/SharedKernel/Result.cs` | Railway-oriented result container. Success carries a value; failure carries an `Error`. Implicit conversion: a non-null `T` value auto-converts to `Result<T>` success. |
| `Error` | `src/SharedKernel/Error.cs` | Immutable record with `Type` (error code string), `Description`, `ErrorCategory` (enum), `Code` (HttpStatusCode), and `Data` (dictionary). Factory methods: `NotFound()`, `Failure()`, `Problem()`, `Conflict()`. |
| `ErrorCategory` | `src/SharedKernel/ErrorCategory.cs` | Enum: `Failure`, `Validation`, `Problem`, `NotFound`, `Conflict`. |
| `ValidationError` | `src/SharedKernel/ValidationError.cs` | Sealed record extending `Error` for batch validation failures. Constructed from an array of `Error[]`. |
| `BaseErrors<T>` | `src/SharedKernel/BaseErrors.cs` | Abstract generic base for domain-specific error classes. Sets `ErrorGroup` to the type name. Provides `NotFound(id)`, `AlreadyExists(id)`, `Failure(type, msg)`, `Conflict(type, msg)`, `Problem(type, msg)`. |
| `IIdentifiable<T>` | `src/SharedKernel/IIdentifiable.cs` | Interface requiring `T Id { get; set; }`. All domain entities must implement this. |

---

## Layer: Domain

| Property | Value |
|---|---|
| Path | `src/Domain/` |
| Namespace | `Clean.Architecture.Template.Domain` |
| References | SharedKernel |

### Rules

- Entities are simple POCOs implementing `IIdentifiable<T>`.
- No framework dependencies. No EF attributes, no data annotations.
- One entity per file. File name matches class name.

### Reference Implementation

**`src/Domain/DummyItem.cs`** — canonical entity example:
```csharp
using Clean.Architecture.Template.SharedKernel;

namespace Clean.Architecture.Template.Domain;

public class DummyItem : IIdentifiable<Guid>
{
    public Guid Id { get; set; }
    public string Name { get; set; } = null!;
}
```

---

## Layer: Application

| Property | Value |
|---|---|
| Path | `src/Application/` |
| Namespace | `Clean.Architecture.Template.Application` |
| References | Domain |
| Key packages | `Mediator.Abstractions` (3.0.2), `Mediator.SourceGenerator` (3.0.2), `Facet` (6.0.1), `Microsoft.Extensions.Logging` (10.0.5) |

### Folder Convention

```
src/Application/
├── Abstractions/
│   └── Repositories/          # Repository interfaces (IAsyncRepository<T,TX>, per-entity interfaces)
├── Behaviours/                # Pipeline behaviors (LoggingBehaviour, etc.)
├── Features/
│   └── {EntityName}/          # One folder per aggregate/entity
│       ├── {Entity}Dtos.cs           # Facet-generated DTOs (request/response records)
│       ├── {Entity}Errors.cs         # Domain-specific errors (BaseErrors<T>)
│       ├── Create{Entity}Command.cs
│       ├── Get{Entity}Query.cs
│       └── ...
└── ApplicationExtensions.cs   # DI registration via AddCoreApplication()
```

### Command/Query Pattern

Commands are **sealed records** implementing `ICommand<Result<T>>`. Handlers are **sealed classes** using **primary constructors** for dependency injection, implementing `ICommandHandler<TCommand, Result<T>>`.

**`src/Application/Features/DummyItems/CreateDummyItemCommand.cs`** — canonical example:
```csharp
using Clean.Architecture.Template.Application.Abstractions.Repositories;
using Clean.Architecture.Template.Domain;
using Clean.Architecture.Template.SharedKernel;
using Mediator;

namespace Clean.Architecture.Template.Application.Features.DummyItems;

public sealed record CreateDummyItemCommand(string Name) : ICommand<Result<DummyItemResponse>>;

public sealed class CreateDummyItemCommandHandler(IDummyItemRepository dummyItemRepository)
    : ICommandHandler<CreateDummyItemCommand, Result<DummyItemResponse>>
{
    public async ValueTask<Result<DummyItemResponse>> Handle(
        CreateDummyItemCommand request, CancellationToken cancellationToken)
    {
        var entity = new DummyItem();
        entity.Name = request.Name;
        await dummyItemRepository.AddAsync(entity, cancellationToken);
        return new DummyItemResponse(entity);
    }
}
```

### DTO Pattern (Facet)

DTOs are **partial records** annotated with `[Facet(typeof(Entity))]`. Facet source-generates constructors and projection expressions at compile time.

**`src/Application/Features/DummyItems/DummyItemDtos.cs`** — canonical example:
```csharp
using Clean.Architecture.Template.Domain;
using Facet;

namespace Clean.Architecture.Template.Application.Features.DummyItems;

[Facet(typeof(DummyItem), exclude: [nameof(DummyItem.Id)])]
public partial record CreateDummyItemRequest;

[Facet(typeof(DummyItem))]
public partial record DummyItemResponse;
```

**Rules**: Handlers return DTOs (e.g. `Result<DummyItemResponse>`), never domain entities. Use `new DummyItemResponse(entity)` to map.

### Repository Interfaces

- Generic base: `IAsyncRepository<T, TX>` in `src/Application/Abstractions/Repositories/IAsyncRepository.cs`
  - `T` = entity type (must implement `IIdentifiable<TX>`)
  - `TX` = ID type (must implement `IEquatable<TX>`)
  - Methods: `GetByIdAsync`, `GetAsQueryable`, `ListAllAsync`, `AddAsync`, `UpdateAsync`, `DeleteAsync`, `GetPagedResponseAsync`
  - All async methods return `ValueTask`.
- Per-entity interfaces extend the generic base:
  ```csharp
  public interface IDummyItemRepository : IAsyncRepository<DummyItem, Guid> { }
  ```

### Mediator Registration

DI is configured in `src/Application/ApplicationExtensions.cs` via `services.AddCoreApplication()`:
- Service lifetime: **Scoped**
- Pipeline behaviors: `LoggingBehaviour<,>` (logs all requests, differentiates NotFound vs other failures)
- Notification publisher: `ForeachAwaitPublisher` (sequential domain event processing)
- Assemblies scanned: the Application assembly

### Pipeline Behavior

`src/Application/Behaviours/LoggingBehaviour.cs` implements `IPipelineBehavior<TMessage, TResponse>` where `TResponse : Result`. It:
- Logs message type and parameters before execution
- Logs `Information` for `NotFound` failures, `Error` for all other failures
- Re-throws exceptions (does not swallow them)

---

## Layer: Api (Presentation)

| Property | Value |
|---|---|
| Path | `src/Api/` |
| Namespace | `Clean.Architecture.Template.Api` |
| References | Application, Infrastructure |
| Key packages | `Mediator.Abstractions` (3.0.2), `Microsoft.AspNetCore.OpenApi` (10.0.5), Serilog ecosystem |
| Ports | HTTP: 5282, HTTPS: 7272 |

### Controller Pattern

Controllers inject `IMediator` via primary constructor. Every action dispatches a command/query and pipes the result through `.ToHttpResponse()`.

**`src/Api/Controllers/DummyItemsController.cs`** — canonical example:
```csharp
using Clean.Architecture.Template.Api.Extensions;
using Clean.Architecture.Template.Application.Features.DummyItems;
using Mediator;
using Microsoft.AspNetCore.Mvc;

namespace Clean.Architecture.Template.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class DummyItemsController(IMediator mediator) : ControllerBase
{
    [HttpPost]
    public async Task<IActionResult> Create(CreateDummyItemCommand command)
        => await mediator.Send(command).ToHttpResponse();
}
```

### Result → HTTP Response Mapping

`src/Api/Extensions/ResultExtensions.cs` provides `.ToHttpResponse()` extension methods:
- **Success** → `200 OK` with `result.Value` as body
- **Failure** → HTTP status from `Error.Code` (HttpStatusCode) with the `Error` object as body

Overloads exist for `Result`, `Result<T>`, `ValueTask<Result>`, and `ValueTask<Result<T>>`.

### Global Exception Handler

`src/Api/Middleware/GlobalExceptionHandler.cs` implements `IExceptionHandler`. Catches unhandled exceptions and returns a `ProblemDetails` JSON response with status 500.

### Startup Wiring (`src/Api/Program.cs`)

1. Serilog configured with structured logging, `SourceContext` enrichment, and exception destructuring. Sink config in `appsettings.json` / `appsettings.Development.json`.
2. `.env` file loaded in non-production environments
3. `builder.Services.AddCoreApplication()` registers the Application layer (Mediator + behaviors)
4. `builder.Services.AddInfrastructure(builder.Configuration)` registers EF Core + repositories
5. `AddOpenApi()` + `MapOpenApi()` (dev only) for OpenAPI document generation
6. `AddExceptionHandler<GlobalExceptionHandler>()` + `AddProblemDetails()` for structured error responses
7. `AddHealthChecks()` + `MapHealthChecks("/health")` for health monitoring
8. `AddCors()` with `AllowAnyOrigin/Method/Header` default policy (customize before production)
9. Middleware order: `UseExceptionHandler()` → `UseHttpsRedirection()` → `UseCors()` → `UseAuthorization()` → endpoints

---

## Layer: Infrastructure

| Property | Value |
|---|---|
| Path | `src/Infrastructure/` |
| Namespace | `Clean.Architecture.Template.Infrastructure` |
| References | Application, Domain, SharedKernel |
| Key packages | `Microsoft.EntityFrameworkCore` (10.0.5), `Microsoft.EntityFrameworkCore.Sqlite` (10.0.5) |
| Purpose | EF Core DbContext, repository implementations, external service clients |

### Key Files

| File | Purpose |
|---|---|
| `Persistence/ApplicationDbContext.cs` | EF Core DbContext. Applies configurations from assembly. |
| `Persistence/Configurations/DummyItemConfiguration.cs` | Fluent EF config for DummyItem (key, Name max-length 200). |
| `Repositories/BaseRepository.cs` | Generic `IAsyncRepository<T, TX>` implementation using EF Core. All methods return `ValueTask`. |
| `Repositories/DummyItemRepository.cs` | `DummyItemRepository(ApplicationDbContext) : BaseRepository<DummyItem, Guid>, IDummyItemRepository` |
| `InfrastructureExtensions.cs` | `AddInfrastructure(IConfiguration)` — registers DbContext (SQLite) and scoped repositories. |

### Adding a New Repository

1. Create `src/Infrastructure/Repositories/{EntityName}Repository.cs` inheriting `BaseRepository<{Entity}, {IdType}>` and implementing `I{EntityName}Repository`.
2. Register it in `InfrastructureExtensions.cs` as scoped.

---

## Tests

| Property | Value |
|---|---|
| Path | `tests/Application.Tests/` |
| Framework | xUnit, NSubstitute (5.3.0) |
| Convention | One test class per handler, file at `Features/{EntityName}/{HandlerName}Tests.cs` |

---

## Husky Pre-Commit Hooks

Configured in `.husky/task-runner.json`. Runs `dotnet build` and `dotnet test` on pre-commit. Husky is installed via `dotnet tool restore` + `dotnet husky install`.

---

## How To: Add a New Entity

1. **Domain** — Create `src/Domain/{EntityName}.cs`. Implement `IIdentifiable<Guid>` (or appropriate ID type). Keep it a simple POCO.
2. **Application/Repository** — Create `src/Application/Abstractions/Repositories/I{EntityName}Repository.cs` extending `IAsyncRepository<{EntityName}, Guid>`.
3. **Application/Feature** — Create folder `src/Application/Features/{EntityName}s/`. Add command/query records and handlers (see Command/Query Pattern above).
4. **Application/Errors** (optional) — Create a `{EntityName}Errors.cs` class inheriting `BaseErrors<{EntityName}>` with domain-specific error factories.
5. **Api/Controller** — Create `src/Api/Controllers/{EntityName}sController.cs`. Inject `IMediator`, dispatch commands, return `.ToHttpResponse()`.
6. **Infrastructure** — Create `src/Infrastructure/Repositories/{EntityName}Repository.cs` inheriting `BaseRepository` and implementing the repository interface. Add EF config in `Persistence/Configurations/`. Register in `InfrastructureExtensions.cs`.
7. **Tests** — Create `tests/Application.Tests/Features/{EntityName}s/` with handler test classes using xUnit + NSubstitute.

## How To: Add a New Command

1. Create a **sealed record** in `src/Application/Features/{EntityName}/`:
   ```csharp
   public sealed record DoSomethingCommand(/* parameters */) : ICommand<Result<ResponseType>>;
   ```
2. Create a **sealed handler class** in the same file (or a separate file):
   ```csharp
   public sealed class DoSomethingCommandHandler(/* dependencies */)
       : ICommandHandler<DoSomethingCommand, Result<ResponseType>>
   {
       public async ValueTask<Result<ResponseType>> Handle(
           DoSomethingCommand request, CancellationToken cancellationToken)
       {
           // business logic
           // return value; (implicit success) or return Result.Failure<T>(error);
       }
   }
   ```
3. Add an endpoint in the appropriate controller that calls `mediator.Send(command).ToHttpResponse()`.

## How To: Add Domain-Specific Errors

Create a static error class inheriting `BaseErrors<T>`:

```csharp
public class {EntityName}Errors : BaseErrors<{EntityName}>
{
    // Inherited: NotFound(), NotFound(id), AlreadyExists(), AlreadyExists(id),
    //           Failure(type, msg), Conflict(type, msg), Problem(type, msg)

    // Add entity-specific errors:
    public static Error SomeSpecificError()
        => Failure("SomeSpecificError", "Description of what went wrong");
}
```

Error codes follow the pattern `{ErrorGroup}.{Type}` where `ErrorGroup` defaults to the entity type name.
