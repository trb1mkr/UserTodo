namespace UserTodo.Api.Dtos.Api;

public class TodoWithUserResponse
{
    public int Id { get; set; }
    public string Todo { get; set; } = string.Empty;
    public bool Completed { get; set; }
    public UserResponse User { get; set; } = null!;
}
