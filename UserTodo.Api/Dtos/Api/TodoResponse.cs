namespace UserTodo.Api.Dtos.Api;

public class TodoResponse
{
    public int Id { get; set; }
    public string Todo { get; set; } = string.Empty;
    public bool Completed { get; set; }
}
