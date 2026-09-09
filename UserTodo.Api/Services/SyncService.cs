using UserTodo.Api.Data;
using UserTodo.Api.Models;

namespace UserTodo.Api.Services;

public class SyncService
{
    private readonly ApplicationDbContext _db;
    private readonly DummyJsonClient _client;

    public SyncService(ApplicationDbContext db, DummyJsonClient client)
    {
        _db = db;
        _client = client;
    }

    public async Task SyncAsync(CancellationToken ct)
    {
        var users = await _client.GetUsersAsync(ct);

        foreach (var dto in users)
        {
            var existing = await _db.Users.FindAsync(new object[] { dto.Id }, ct);
            if (existing is null)
            {
                _db.Users.Add(new User
                {
                    Id = dto.Id,
                    FirstName = dto.FirstName,
                    LastName = dto.LastName,
                    Email = dto.Email,
                    Username = dto.Username
                });
            }
            else
            {
                existing.FirstName = dto.FirstName;
                existing.LastName = dto.LastName;
                existing.Email = dto.Email;
                existing.Username = dto.Username;
            }
        }

        await _db.SaveChangesAsync(ct);

        var todos = await _client.GetTodosAsync(ct);

        foreach (var dto in todos)
        {
            var existing = await _db.Todos.FindAsync(new object[] { dto.Id }, ct);
            if (existing is null)
            {
                _db.Todos.Add(new Todo
                {
                    Id = dto.Id,
                    Description = dto.Todo,
                    Completed = dto.Completed,
                    UserId = dto.UserId
                });
            }
            else
            {
                existing.Description = dto.Todo;
                existing.Completed = dto.Completed;
                existing.UserId = dto.UserId;
            }
        }

        await _db.SaveChangesAsync(ct);
    }
}
