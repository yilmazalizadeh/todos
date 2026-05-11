using System.Text.Json.Serialization;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.EntityFrameworkCore;
using TravelApp;
using TravelApp.Entity;

const string connectionString = "Data Source=travel.db";

var builder = WebApplication.CreateSlimBuilder(args);

builder.Services.AddDbContext<TravelDbContext>(options =>
    options.UseSqlite(connectionString));

builder.Services.AddScoped<TodosService>();

builder.Services.ConfigureHttpJsonOptions(options =>
{
    options.SerializerOptions.TypeInfoResolverChain.Insert(0, AppJsonSerializerContext.Default);
});

// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddOpenApi();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<TravelDbContext>();
    db.Database.EnsureCreated();
}

if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
    app.UseSwagger();
    app.UseSwaggerUI();
}

var todosGroup = app.MapGroup("/todos");

todosGroup.MapGet("/", async (TodosService todosService) => 
    await todosService.GetTodos())
    .WithName("GetTodos");

todosGroup.MapGet("/{id:int}", async Task<Results<Ok<Todo>, NotFound>> (int id, TodosService todosService) =>
        await todosService.GetTodoById(id) is { } todo
            ? TypedResults.Ok(todo)
            : TypedResults.NotFound())
    .WithName("GetTodoById");

todosGroup.MapPost("/", async (Todo todo, TodosService todosService) =>
{
    await todosService.CreateTodo(todo);
    return TypedResults.Created($"/todos/{todo.Id}", todo);
})
.WithName("CreateTodo");

todosGroup.MapPut("/{id:int}", async (int id, Todo todo, TodosService todosService) =>
{
    if (id != todo.Id) return Results.BadRequest();
    await todosService.UpdateTodo(todo);
    return Results.NoContent();
})
.WithName("UpdateTodo");

todosGroup.MapDelete("/{id:int}", async (int id, TodosService todosService) =>
{
    await todosService.DeleteTodo(id);
    return Results.NoContent();
})
.WithName("DeleteTodo");

app.Run();

[JsonSerializable(typeof(List<Todo>))]
[JsonSerializable(typeof(Todo))]
internal partial class AppJsonSerializerContext : JsonSerializerContext
{
}
