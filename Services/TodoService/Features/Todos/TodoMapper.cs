namespace TodoService.Features.Todos;

public static class TodoMapper
{
    public static TodoDto ToDto(Todo todo)
    {
        return new TodoDto(
            todo.Id,
            todo.Title,
            todo.DueBy,
            todo.IsComplete);
    }

    public static Todo ToEntity(CreateTodoDto todoDto)
    {
        return new Todo
        {
            Title = todoDto.Title ?? string.Empty,
            DueBy = todoDto.DueBy,
            IsComplete = todoDto.IsComplete
        };
    }

    public static void ApplyToEntity(UpdateTodoDto todoDto, Todo todo)
    {
        todo.Title = todoDto.Title ?? string.Empty;
        todo.DueBy = todoDto.DueBy;
        todo.IsComplete = todoDto.IsComplete;
    }
}
