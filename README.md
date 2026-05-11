# TravelApp

TravelApp is a small .NET 10 minimal API microservice. It uses SQLite for persistence, Entity Framework Core for data access, Swagger/OpenAPI for API discovery, and a vertical-slice folder structure for feature code.

## Running the Service

Restore, build, and test:

```powershell
dotnet restore
dotnet build
dotnet test
```

Run the API:

```powershell
dotnet run --project TravelApp
```

In development, Swagger UI is available at:

```text
/swagger
```

The application creates `travel.db` automatically at startup through `Database.EnsureCreated()`.

## Project Structure

```text
TravelApp/
  Program.cs
  AppJsonSerializerContext.cs
  TravelDbContext.cs

  Common/
    Dtos/
      ErrorResponse.cs
    Exceptions/
      TravelAppExceptions.cs
    Middleware/
      GlobalExceptionHandlingMiddleware.cs

  Features/
    Todos/
      README.md
      Todo.cs
      TodoDtos.cs
      TodoEndpoints.cs
      TodosService.cs
      ITodosService.cs
      TodoValidator.cs
      ITodoValidator.cs
      TodoMapper.cs
```

## Vertical Slice Pattern

Vertical slice architecture groups code by business capability instead of by technical layer. Feature-specific endpoint definitions, DTOs, validation, mapping, entity code, and service logic live together under `Features/<FeatureName>`.

Cross-cutting code that is not specific to a single feature lives under `Common`.

## Root-Level Files

`Program.cs`

Configures dependency injection, EF Core SQLite, source-generated JSON serialization, Swagger, global exception middleware, and feature endpoint registration.

`TravelDbContext.cs`

Defines the EF Core database context and registered feature tables.

`AppJsonSerializerContext.cs`

Registers API DTO types for `System.Text.Json` source generation.

## Common

`Common/Dtos/ErrorResponse.cs`

Defines the shared JSON error response contract.

`Common/Exceptions/TravelAppExceptions.cs`

Defines the base application exception and shared exception types.

`Common/Middleware/GlobalExceptionHandlingMiddleware.cs`

Catches unhandled exceptions globally and converts known application exceptions into HTTP responses.

## Features

Feature-specific documentation lives with each feature slice.

- [Todos](TravelApp/Features/Todos/README.md)
