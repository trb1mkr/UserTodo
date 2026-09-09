using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;

namespace UserTodo.Api.Models;

public class Todo
{
    public int Id { get; set; }

    // В задании поле называется Todo, но C# запрещает совпадение имени члена с именем типа (CS0542)
    [Column("Todo")]
    [JsonPropertyName("todo")]
    public string Description { get; set; } = string.Empty;

    public bool Completed { get; set; }

    public int UserId { get; set; }

    public User User { get; set; } = null!;
}
