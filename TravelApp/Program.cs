using Microsoft.AspNetCore.Routing.Constraints;
using Microsoft.EntityFrameworkCore;
using TravelApp;
using TravelApp.Common.Middleware;
using TravelApp.Features.Todos;

var builder = WebApplication.CreateSlimBuilder(args);
var connectionString = builder.Configuration.GetConnectionString("TravelDb")
    ?? "Host=localhost;Port=5432;Database=travelapp;Username=travelapp;Password=travelapp_password";

builder.Services.AddDbContext<TravelDbContext>(options =>
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
    var db = scope.ServiceProvider.GetRequiredService<TravelDbContext>();
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
