using Microsoft.EntityFrameworkCore;
using TravelApp.Entity;

namespace TravelApp;

public sealed class TodosService(TravelDbContext db)
{

    public async Task<List<Todo>> GetTodos()
    {
        return await db.Todos
            .OrderBy(todo => todo.Id)
            .ToListAsync();
    }

    public async Task<Todo?> GetTodoById(int id)
    {
        return await db.Todos
            .FirstOrDefaultAsync(todo => todo.Id == id);
    }

    public async Task<Todo> CreateTodo(Todo todo)
    {
        db.Todos.Add(todo);
        await db.SaveChangesAsync();
        return todo;
    }

    public async Task UpdateTodo(Todo todo)
    {
        db.Todos.Update(todo);
        await db.SaveChangesAsync();
    }

    public async Task DeleteTodo(int id)
    {
        var todo = await db.Todos
            .FirstOrDefaultAsync(todo => todo.Id == id);

        if (todo is null)
        {
            return;
        }

        db.Todos.Remove(todo);
        await db.SaveChangesAsync();
    }
}