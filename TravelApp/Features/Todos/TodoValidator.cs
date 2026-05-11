using TravelApp.Entity;
using TravelApp.Exceptions;

namespace TravelApp.Features.Todos;

public sealed class TodoValidator : ITodoValidator
{
    public void Validate(CreateTodoDto todoDto)
    {
        ValidateTitle(todoDto.Title);
    }

    public void Validate(UpdateTodoDto todoDto)
    {
        ValidateTitle(todoDto.Title);
    }

    private static void ValidateTitle(string? title)
    {
        if (string.IsNullOrWhiteSpace(title))
        {
            throw new ValidationException("Todo title cannot be empty.");
        }

        if (title.Length > Todo.MaxTitleLength)
        {
            throw new ValidationException($"Todo title cannot exceed {Todo.MaxTitleLength} characters.");
        }
    }
}
