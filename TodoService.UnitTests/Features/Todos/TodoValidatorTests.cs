using Microsoft.EntityFrameworkCore;
using TodoService.Common.Exceptions;
using TodoService.Features.Todos;

namespace TodoService.UnitTests.Features.Todos;

public class TodoValidatorTests
{
    private static TodoDbContext GetDbContext()
    {
        var options = new DbContextOptionsBuilder<TodoDbContext>()
            .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
            .Options;

        return new TodoDbContext(options);
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
