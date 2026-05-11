using Microsoft.EntityFrameworkCore;
using TravelApp.Dtos;
using TravelApp.Entity;
using TravelApp.Exceptions;

namespace TravelApp;

public sealed class TodosService(TravelDbContext db)
{

    public async Task<List<Todo>> GetTodos()
    {
        return await db.Todos
            .OrderBy(todo => todo.Id)
            .ToListAsync();
    }

    public async Task<Todo> GetTodoById(int id)
    {
        var todo = await db.Todos
            .SingleOrDefaultAsync(todo => todo.Id == id);

        return todo ?? throw new NotFoundException($"Todo with ID {id} was not found.");
    }

    public async Task<Todo> CreateTodo(CreateTodoDto todoDto)
    {
        var todo = new Todo
        {
            Title = todoDto.Title,
            DueBy = todoDto.DueBy,
            IsComplete = todoDto.IsComplete
        };

        ValidateTodo(todo);

        db.Todos.Add(todo);
        await db.SaveChangesAsync();
        return todo;
    }

    public async Task UpdateTodo(int id, UpdateTodoDto todoDto)
    {
        var todo = await db.Todos.SingleOrDefaultAsync(t => t.Id == id);
        if (todo is null)
        {
            throw new NotFoundException($"Todo with ID {id} was not found.");
        }

        todo.Title = todoDto.Title;
        todo.DueBy = todoDto.DueBy;
        todo.IsComplete = todoDto.IsComplete;

        ValidateTodo(todo);

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

    private static void ValidateTodo(Todo todo)
    {
        if (string.IsNullOrWhiteSpace(todo.Title))
        {
            throw new ValidationException("Todo title cannot be empty.");
        }

        if (todo.Title.Length > Todo.MaxTitleLength)
        {
            throw new ValidationException($"Todo title cannot exceed {Todo.MaxTitleLength} characters.");
        }
    }
}
