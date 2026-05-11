using Microsoft.EntityFrameworkCore;

public class TravelDbContext : DbContext
{
    public TravelDbContext(DbContextOptions<TravelDbContext> options) : base(options)
    {
    }

    public DbSet<Todo> Todos => Set<Todo>();
}
