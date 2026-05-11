using Microsoft.EntityFrameworkCore;
using TodoService.Features.Todos;

namespace TodoService;

public class TodoDbContext(DbContextOptions<TodoDbContext> options) : DbContext(options)
{
    public DbSet<Todo> Todos => Set<Todo>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Todo>()
            .Property(todo => todo.Title)
            .HasMaxLength(Todo.MaxTitleLength);
    }
}
