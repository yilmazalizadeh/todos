namespace TravelApp.Dtos;

public sealed record CreateTodoDto(
    string? Title,
    DateOnly? DueBy = null,
    bool IsComplete = false);

public sealed record UpdateTodoDto(
    string? Title,
    DateOnly? DueBy = null,
    bool IsComplete = false);
