using System.Text.Json.Serialization;
using Microsoft.AspNetCore.Http.HttpResults;

using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateSlimBuilder(args);

builder.Services.AddDbContext<TravelDbContext>(options =>
    options.UseSqlite("Data Source=travel.db"));

builder.Services.AddScoped<TodosApi>();

builder.Services.ConfigureHttpJsonOptions(options =>
{
    options.SerializerOptions.TypeInfoResolverChain.Insert(0, AppJsonSerializerContext.Default);
});

// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddOpenApi();

var app = builder.Build();

using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<TravelDbContext>();
    db.Database.EnsureCreated();
}

if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

var todosGroup = app.MapGroup("/todos");

todosGroup.MapGet("/", async (TodosApi todosApi) => 
    await todosApi.GetTodos())
    .WithName("GetTodos");

todosGroup.MapGet("/{id}", async (int id, TodosApi todosApi) =>
        await todosApi.GetTodoById(id) is { } todo
            ? TypedResults.Ok(todo)
            : TypedResults.NotFound())
    .WithName("GetTodoById");

todosGroup.MapPost("/", async (Todo todo, TodosApi todosApi) =>
{
    await todosApi.CreateTodo(todo);
    return TypedResults.Created($"/todos/{todo.Id}", todo);
})
.WithName("CreateTodo");

todosGroup.MapPut("/{id}", async (int id, Todo todo, TodosApi todosApi) =>
{
    if (id != todo.Id) return Results.BadRequest();
    await todosApi.UpdateTodo(todo);
    return Results.NoContent();
})
.WithName("UpdateTodo");

todosGroup.MapDelete("/{id}", async (int id, TodosApi todosApi) =>
{
    await todosApi.DeleteTodo(id);
    return Results.NoContent();
})
.WithName("DeleteTodo");

app.Run();

[JsonSerializable(typeof(List<Todo>))]
[JsonSerializable(typeof(Todo))]
internal partial class AppJsonSerializerContext : JsonSerializerContext
{
}
