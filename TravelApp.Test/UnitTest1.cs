using Microsoft.EntityFrameworkCore;
using TravelApp.Entity;
using TravelApp.Exceptions;
using TravelApp.Features.Todos;

namespace TravelApp.Test;

public class UnitTest1
{
    private static TravelDbContext GetDbContext()
    {
        var options = new DbContextOptionsBuilder<TravelDbContext>()
            .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
            .Options;

        return new TravelDbContext(options);
    }

    [Fact]
    public async Task CRUD_Operations_Work()
    {
        await using var db = GetDbContext();
        var todosApi = new TodosService(db, new TodoValidator());

        // Create
        var newTodo = new CreateTodoDto("Test Todo");
        var created = await todosApi.CreateTodo(newTodo);
        Assert.NotEqual(0, created.Id);

        // Read
        var fetched = await todosApi.GetTodoById(created.Id);
        Assert.Equal("Test Todo", fetched.Title);

        // Update
        await todosApi.UpdateTodo(fetched.Id, new UpdateTodoDto("Updated Title", fetched.DueBy, fetched.IsComplete));
        var updated = await todosApi.GetTodoById(created.Id);
        Assert.Equal("Updated Title", updated.Title);

        // Delete
        await todosApi.DeleteTodo(created.Id);
        await Assert.ThrowsAsync<NotFoundException>(() => todosApi.GetTodoById(created.Id));
    }

    [Fact]
    public async Task GetTodoById_Throws_WhenTodoDoesNotExist()
    {
        await using var db = GetDbContext();
        var todosApi = new TodosService(db, new TodoValidator());

        var exception = await Assert.ThrowsAsync<NotFoundException>(() => todosApi.GetTodoById(123));

        Assert.Equal("Todo with ID 123 was not found.", exception.Message);
    }

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData("   ")]
    public async Task CreateTodo_Throws_WhenTitleIsEmpty(string? title)
    {
        await using var db = GetDbContext();
        var todosApi = new TodosService(db, new TodoValidator());

        var exception = await Assert.ThrowsAsync<ValidationException>(() =>
            todosApi.CreateTodo(new CreateTodoDto(title)));

        Assert.Equal("Todo title cannot be empty.", exception.Message);
    }

    [Fact]
    public async Task CreateTodo_Throws_WhenTitleIsTooLong()
    {
        await using var db = GetDbContext();
        var todosApi = new TodosService(db, new TodoValidator());
        var title = new string('A', Todo.MaxTitleLength + 1);

        var exception = await Assert.ThrowsAsync<ValidationException>(() =>
            todosApi.CreateTodo(new CreateTodoDto(title)));

        Assert.Equal($"Todo title cannot exceed {Todo.MaxTitleLength} characters.", exception.Message);
    }
}
