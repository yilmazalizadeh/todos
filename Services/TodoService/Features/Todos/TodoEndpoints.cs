using Microsoft.AspNetCore.Http.HttpResults;

namespace TodoService.Features.Todos;

public static class TodoEndpoints
{
    public static IEndpointRouteBuilder MapTodoEndpoints(this IEndpointRouteBuilder app)
    {
        var todosGroup = app.MapGroup("/todos");

        todosGroup.MapGet("/", async Task<Ok<List<TodoDto>>> (ITodosService todosService) =>
            TypedResults.Ok(await todosService.GetTodos()))
            .WithName("GetTodos");

        todosGroup.MapGet("/{id:int}", async Task<Ok<TodoDto>> (int id, ITodosService todosService) =>
            TypedResults.Ok(await todosService.GetTodoById(id)))
            .WithName("GetTodoById");

        todosGroup.MapPost("/", async Task<Created<TodoDto>> (CreateTodoDto todo, ITodosService todosService) =>
        {
            var createdTodo = await todosService.CreateTodo(todo);
            return TypedResults.Created($"/todos/{createdTodo.Id}", createdTodo);
        })
        .WithName("CreateTodo");

        todosGroup.MapPut("/{id:int}", async Task<NoContent> (int id, UpdateTodoDto todo, ITodosService todosService) =>
        {
            await todosService.UpdateTodo(id, todo);
            return TypedResults.NoContent();
        })
        .WithName("UpdateTodo");

        todosGroup.MapDelete("/{id:int}", async Task<NoContent> (int id, ITodosService todosService) =>
        {
            await todosService.DeleteTodo(id);
            return TypedResults.NoContent();
        })
        .WithName("DeleteTodo");

        return app;
    }
}
