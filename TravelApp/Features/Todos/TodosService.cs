using Microsoft.EntityFrameworkCore;
using TravelApp.Exceptions;

namespace TravelApp.Features.Todos;

public sealed class TodosService(TravelDbContext db, ITodoValidator validator) : ITodosService
{
    public async Task<List<TodoDto>> GetTodos()
    {
        var todos = await db.Todos
            .AsNoTracking()
            .OrderBy(todo => todo.Id)
            .ToListAsync();

        return todos.ConvertAll(TodoMapper.ToDto);
    }

    public async Task<TodoDto> GetTodoById(int id)
    {
        var todo = await db.Todos
            .AsNoTracking()
            .SingleOrDefaultAsync(todo => todo.Id == id);

        return todo is null
            ? throw new NotFoundException($"Todo with ID {id} was not found.")
            : TodoMapper.ToDto(todo);
    }

    public async Task<TodoDto> CreateTodo(CreateTodoDto todoDto)
    {
        validator.Validate(todoDto);

        var todo = TodoMapper.ToEntity(todoDto);
        db.Todos.Add(todo);
        await db.SaveChangesAsync();

        return TodoMapper.ToDto(todo);
    }

    public async Task UpdateTodo(int id, UpdateTodoDto todoDto)
    {
        var todo = await db.Todos.SingleOrDefaultAsync(t => t.Id == id);
        if (todo is null)
        {
            throw new NotFoundException($"Todo with ID {id} was not found.");
        }

        validator.Validate(todoDto);

        TodoMapper.ApplyToEntity(todoDto, todo);

        await db.SaveChangesAsync();
    }

    public async Task DeleteTodo(int id)
    {
        var todo = await db.Todos
            .SingleOrDefaultAsync(todo => todo.Id == id);

        if (todo is null)
        {
            throw new NotFoundException($"Todo with ID {id} was not found.");
        }

        db.Todos.Remove(todo);
        await db.SaveChangesAsync();
    }
}
