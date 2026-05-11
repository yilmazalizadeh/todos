using Microsoft.EntityFrameworkCore;

public sealed class TodosApi
{
    private readonly TravelDbContext db;

    public TodosApi(TravelDbContext db)
    {
        this.db = db;
    }

    public async Task<List<Todo>> GetTodos()
    {
        return await db.Todos.ToListAsync();
    }

    public async Task<Todo?> GetTodoById(int id)
    {
        return await db.Todos.FindAsync(id);
    }

    public async Task<Todo> CreateTodo(Todo todo)
    {
        db.Todos.Add(todo);
        await db.SaveChangesAsync();
        return todo;
    }

    public async Task UpdateTodo(Todo todo)
    {
        db.Entry(todo).State = EntityState.Modified;
        await db.SaveChangesAsync();
    }

    public async Task DeleteTodo(int id)
    {
        var todo = await db.Todos.FindAsync(id);
        if (todo != null)
        {
            db.Todos.Remove(todo);
            await db.SaveChangesAsync();
        }
    }
}

public class Todo
{
    public int Id { get; set; }
    public string? Title { get; set; }
    public DateOnly? DueBy { get; set; }
    public bool IsComplete { get; set; }

    public Todo() { }
    public Todo(int id, string? title, DateOnly? dueBy = null, bool isComplete = false)
    {
        Id = id;
        Title = title;
        DueBy = dueBy;
        IsComplete = isComplete;
    }
}
