using UserTodo.Api.Dtos.DummyJson;

namespace UserTodo.Api.Services;

public class DummyJsonClient
{
    private readonly HttpClient _http;

    public DummyJsonClient(HttpClient http)
    {
        _http = http;
    }

    public async Task<List<DummyUserDto>> GetUsersAsync(CancellationToken ct)
    {
        var response = await _http.GetFromJsonAsync<DummyUsersResponse>("/users?limit=0", ct);
        return response?.Users ?? new List<DummyUserDto>();
    }

    public async Task<List<DummyTodoDto>> GetTodosAsync(CancellationToken ct)
    {
        var response = await _http.GetFromJsonAsync<DummyTodosResponse>("/todos?limit=0", ct);
        return response?.Todos ?? new List<DummyTodoDto>();
    }
}
