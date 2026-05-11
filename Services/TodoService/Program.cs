using Microsoft.AspNetCore.Routing.Constraints;
using Microsoft.EntityFrameworkCore;
using TodoService;
using TodoService.Common.Middleware;
using TodoService.Features.Todos;

var builder = WebApplication.CreateSlimBuilder(args);
var connectionString = builder.Configuration.GetConnectionString("TodoDb")
    ?? "Host=localhost;Port=5432;Database=todoservice;Username=todoservice;Password=todoservice_password";

builder.Services.AddDbContext<TodoDbContext>(options =>
    options.UseNpgsql(connectionString));

builder.Services.Configure<RouteOptions>(options =>
    options.SetParameterPolicy<RegexInlineRouteConstraint>("regex"));

builder.Services.AddScoped<ITodoValidator, TodoValidator>();
builder.Services.AddScoped<ITodosService, TodosService>();

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
    var db = scope.ServiceProvider.GetRequiredService<TodoDbContext>();
    db.Database.EnsureCreated();
}

if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.MapTodoEndpoints();

app.Run();
