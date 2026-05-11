using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Routing.Constraints;
using Microsoft.EntityFrameworkCore;
using TravelApp;
using TravelApp.Dtos;
using TravelApp.Entity;
using TravelApp.Middleware;

const string connectionString = "Data Source=travel.db";

var builder = WebApplication.CreateSlimBuilder(args);

builder.Services.AddDbContext<TravelDbContext>(options =>
    options.UseSqlite(connectionString));

builder.Services.Configure<RouteOptions>(options =>
    options.SetParameterPolicy<RegexInlineRouteConstraint>("regex"));

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

app.UseMiddleware<GlobalExceptionHandlingMiddleware>();

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

todosGroup.MapGet("/{id:int}", async Task<Ok<Todo>> (int id, TodosService todosService) =>
        TypedResults.Ok(await todosService.GetTodoById(id)))
    .WithName("GetTodoById");

todosGroup.MapPost("/", async (CreateTodoDto todo, TodosService todosService) =>
{
    var createdTodo = await todosService.CreateTodo(todo);
    return TypedResults.Created($"/todos/{createdTodo.Id}", createdTodo);
})
.WithName("CreateTodo");

todosGroup.MapPut("/{id:int}", async (int id, UpdateTodoDto todo, TodosService todosService) =>
{
    await todosService.UpdateTodo(id, todo);
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
