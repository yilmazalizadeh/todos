namespace TodoService.Features.Todos;

public interface ITodoValidator
{
    void Validate(CreateTodoDto todoDto);

    void Validate(UpdateTodoDto todoDto);
}
