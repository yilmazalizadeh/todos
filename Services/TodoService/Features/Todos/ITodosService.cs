namespace TodoService.Features.Todos;

public interface ITodosService
{
    Task<List<TodoDto>> GetTodos();

    Task<TodoDto> GetTodoById(int id);

    Task<TodoDto> CreateTodo(CreateTodoDto todoDto);

    Task UpdateTodo(int id, UpdateTodoDto todoDto);

    Task DeleteTodo(int id);
}
