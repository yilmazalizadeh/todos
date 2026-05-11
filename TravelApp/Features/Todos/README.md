# Todos Feature

The Todos feature is implemented as a vertical slice. The feature owns its API contracts, endpoint mapping, validation, mapping, entity, and service behavior.

## Endpoints

```text
GET    /todos
GET    /todos/{id}
POST   /todos
PUT    /todos/{id}
DELETE /todos/{id}
```

## Request Body

Create and update requests use DTOs instead of the EF entity.

```json
{
  "title": "Book hotel",
  "dueBy": "2026-05-20",
  "isComplete": false
}
```

## Response Body

Read endpoints return `TodoDto`.

```json
{
  "id": 1,
  "title": "Book hotel",
  "dueBy": "2026-05-20",
  "isComplete": false
}
```

## Feature Structure

```text
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

## Key Files

`Todo.cs`

The EF Core entity for todo persistence. It defines the persisted fields and the `MaxTitleLength` constraint.

`TodoDtos.cs`

Defines the public API contracts:

- `TodoDto`
- `CreateTodoDto`
- `UpdateTodoDto`

`TodoEndpoints.cs`

Maps all `/todos` HTTP endpoints. The endpoints depend on `ITodosService`, keeping HTTP routing separate from feature behavior.

`ITodosService.cs`

Defines the service contract used by the endpoints.

`TodosService.cs`

Coordinates todo operations. It queries EF Core, delegates validation to `ITodoValidator`, delegates mapping to `TodoMapper`, and throws application exceptions for not-found cases.

`ITodoValidator.cs` and `TodoValidator.cs`

Validate create and update requests. Current rules require a non-empty title and enforce `Todo.MaxTitleLength`.

`TodoMapper.cs`

Converts between request DTOs, response DTOs, and the EF entity.

## Error Handling

The feature throws shared application exceptions:

- `NotFoundException` when a todo does not exist
- `ValidationException` when a request fails validation

`GlobalExceptionHandlingMiddleware` in `Common/Middleware` converts those exceptions into HTTP responses.

Example error response:

```json
{
  "error": "Todo with ID 123 was not found.",
  "type": "NotFoundException"
}
```

## Design Notes

- API write models are separate from EF entities.
- API read responses use `TodoDto`, not the persistence entity.
- Todo-specific code stays in this folder.
- Shared error handling remains in `Common`.
- The service is registered behind `ITodosService` so endpoints are decoupled from the concrete implementation.
