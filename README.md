# TravelApp

TravelApp is a small .NET 10 minimal API microservice. It uses PostgreSQL for persistence, HashiCorp Vault for environment-specific configuration, Entity Framework Core for data access, Nginx as the container ingress proxy, Swagger/OpenAPI for API discovery, and a vertical-slice folder structure for feature code.

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

When running through Docker Compose, PostgreSQL and Vault are started as separate services. Vault is seeded with environment-specific connection strings, and the application creates the database schema at startup through `Database.EnsureCreated()`.

## Container Usage

Production/CI image build:

```powershell
docker build -t travelapp:latest .
```

Run the production-style compose file:

```powershell
docker compose up --build
```

The API is reached through Nginx:

```text
http://localhost:8080
```

Run the test stage in CI:

```powershell
docker build --target test .
```

Run the local development compose file with source mounted and `dotnet watch` enabled:

```powershell
docker compose -f docker-compose.local.yml up --build
```

Select which Vault environment the app reads from:

```powershell
$env:VAULT_ENVIRONMENT="staging"
docker compose up --build
```

Supported seeded paths:

```text
secret/travelapp/dev
secret/travelapp/staging
secret/travelapp/prod
```

Container files:

```text
Dockerfile                 Production/CI multi-stage build
Dockerfile.local           Local development image
docker-compose.yml         Production/CI-style compose file
docker-compose.local.yml   Local development compose file
nginx/default.conf         Nginx reverse proxy config
postgres/init              PostgreSQL database initialization scripts
scripts/docker-entrypoint.sh
                           Loads app configuration from Vault before starting .NET
.dockerignore              Docker build context exclusions
```

Both compose files run these services:

```text
nginx        Publishes the host port and proxies traffic to the API container
travelapp    Runs the .NET API inside the Docker network
postgres     Stores application data in a Docker volume
vault        Stores environment-specific application settings
vault-init   Seeds dev, staging, and prod settings into Vault
```

The app receives Vault coordinates through environment variables:

```text
VAULT_ADDR=http://vault:8200
VAULT_TOKEN=root
VAULT_ENVIRONMENT=prod
```

The entrypoint then loads this key from Vault and exports it for ASP.NET Core configuration:

```text
ConnectionStrings__TravelDb
```

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

Configures dependency injection, EF Core PostgreSQL, source-generated JSON serialization, Swagger, global exception middleware, and feature endpoint registration.

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
