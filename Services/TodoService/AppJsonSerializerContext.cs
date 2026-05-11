using System.Text.Json.Serialization;
using TodoService.Common.Dtos;
using TodoService.Features.Todos;

namespace TodoService;

[JsonSerializable(typeof(ErrorResponse))]
[JsonSerializable(typeof(TodoDto))]
[JsonSerializable(typeof(CreateTodoDto))]
[JsonSerializable(typeof(UpdateTodoDto))]
[JsonSerializable(typeof(List<TodoDto>))]
internal partial class AppJsonSerializerContext : JsonSerializerContext
{
}
