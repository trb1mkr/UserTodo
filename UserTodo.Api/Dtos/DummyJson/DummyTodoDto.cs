using System.Text.Json.Serialization;

namespace UserTodo.Api.Dtos.DummyJson;

public class DummyTodoDto
{
    [JsonPropertyName("id")]
    public int Id { get; set; }

    [JsonPropertyName("todo")]
    public string Todo { get; set; } = string.Empty;

    [JsonPropertyName("completed")]
    public bool Completed { get; set; }

    [JsonPropertyName("userId")]
    public int UserId { get; set; }
}
