using Microsoft.EntityFrameworkCore;
using TravelApp.Features.Todos;

namespace TravelApp;

public class TravelDbContext(DbContextOptions<TravelDbContext> options) : DbContext(options)
{
    public DbSet<Todo> Todos => Set<Todo>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Todo>()
            .Property(todo => todo.Title)
            .HasMaxLength(Todo.MaxTitleLength);
    }
}
