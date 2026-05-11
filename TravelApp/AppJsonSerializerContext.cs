using System.Text.Json.Serialization;
using TravelApp.Dtos;
using TravelApp.Entity;
using TravelApp.Middleware;

namespace TravelApp;

[JsonSerializable(typeof(ErrorResponse))]
[JsonSerializable(typeof(CreateTodoDto))]
[JsonSerializable(typeof(UpdateTodoDto))]
[JsonSerializable(typeof(List<Todo>))]
[JsonSerializable(typeof(Todo))]
internal partial class AppJsonSerializerContext : JsonSerializerContext
{
}
