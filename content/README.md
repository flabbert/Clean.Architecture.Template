# Clean Architecture Template

A .NET 10 solution template following Clean Architecture principles with CQRS, Mediator, and a Result pattern.

## Getting Started

### Prerequisites

- [.NET 10 SDK](https://dotnet.microsoft.com/download)

### Run the application

```powershell
./start.ps1
```

Or manually:

```bash
dotnet watch --project ./src/Api/Api.csproj
```

The API will be available at `https://localhost:7272` / `http://localhost:5282`.

### Run tests

```bash
dotnet test
```

## Project Structure

```
src/
├── Api/              # Presentation layer (controllers, middleware)
├── Application/      # Use cases (commands, queries, repository interfaces)
├── Domain/           # Entities (plain POCOs)
├── SharedKernel/     # Cross-cutting: Result, Error, BaseErrors
└── Infrastructure/   # EF Core, repository implementations
tests/
└── Application.Tests # Unit tests (xUnit + NSubstitute)
```

## Architecture Rules

- **Dependency flow**: Api → Application → Domain → SharedKernel
- **No exceptions for business logic** — use `Result<T>` / `Error`
- **Mediator** (source-generated) for CQRS, not MediatR
- **Facet** for compile-time DTO generation
- **No FluentValidation** — validate in handlers with the Result/Error pattern

## Environment Variables

Copy `.env.example` to `.env` at the solution root and adjust values.

## Health Check

```
GET /health
```
