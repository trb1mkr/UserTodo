using System.Text.Json.Serialization;

namespace UserTodo.Api.Dtos.DummyJson;

public class DummyUsersResponse
{
    [JsonPropertyName("users")]
    public List<DummyUserDto> Users { get; set; } = new();
}

public class DummyTodosResponse
{
    [JsonPropertyName("todos")]
    public List<DummyTodoDto> Todos { get; set; } = new();
}
