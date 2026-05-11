using Microsoft.EntityFrameworkCore;

namespace TravelApp.Test;

public class UnitTest1
{
    private TravelDbContext GetDbContext()
    {
        var options = new DbContextOptionsBuilder<TravelDbContext>()
            .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
            .Options;
        return new TravelDbContext(options);
    }

    [Fact]
    public async Task CRUD_Operations_Work()
    {
        var db = GetDbContext();
        var todosApi = new TodosApi(db);

        // Create
        var newTodo = new Todo { Title = "Test Todo", IsComplete = false };
        var created = await todosApi.CreateTodo(newTodo);
        Assert.NotEqual(0, created.Id);

        // Read
        var fetched = await todosApi.GetTodoById(created.Id);
        Assert.NotNull(fetched);
        Assert.Equal("Test Todo", fetched.Title);

        // Update
        fetched.Title = "Updated Title";
        await todosApi.UpdateTodo(fetched);
        var updated = await todosApi.GetTodoById(created.Id);
        Assert.Equal("Updated Title", updated.Title);

        // Delete
        await todosApi.DeleteTodo(created.Id);
        var deleted = await todosApi.GetTodoById(created.Id);
        Assert.Null(deleted);
    }
}
