namespace TodoService.Features.Todos;

public sealed record TodoDto(
    int Id,
    string Title,
    DateOnly? DueBy,
    bool IsComplete);

public sealed record CreateTodoDto(
    string? Title,
    DateOnly? DueBy = null,
    bool IsComplete = false);

public sealed record UpdateTodoDto(
    string? Title,
    DateOnly? DueBy = null,
    bool IsComplete = false);
