using System.Text.Json.Serialization;
using TravelApp.Dtos;
using TravelApp.Features.Todos;

namespace TravelApp;

[JsonSerializable(typeof(ErrorResponse))]
[JsonSerializable(typeof(TodoDto))]
[JsonSerializable(typeof(CreateTodoDto))]
[JsonSerializable(typeof(UpdateTodoDto))]
[JsonSerializable(typeof(List<TodoDto>))]
internal partial class AppJsonSerializerContext : JsonSerializerContext
{
}
