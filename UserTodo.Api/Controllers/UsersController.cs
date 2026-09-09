using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using UserTodo.Api.Data;
using UserTodo.Api.Dtos.Api;

namespace UserTodo.Api.Controllers;

[ApiController]
[Route("api/users")]
public class UsersController : ControllerBase
{
    private readonly ApplicationDbContext _db;

    public UsersController(ApplicationDbContext db)
    {
        _db = db;
    }

    [HttpGet]
    public async Task<ActionResult<List<UserResponse>>> GetUsers(
        [FromQuery] int limit = 30,
        [FromQuery] int offset = 0,
        [FromQuery] string? firstName = null,
        [FromQuery] string? lastName = null,
        [FromQuery] string? email = null,
        [FromQuery] string? username = null,
        [FromQuery] string sortBy = "id",
        [FromQuery] string orderBy = "asc")
    {
        var query = _db.Users.AsQueryable();

        if (!string.IsNullOrWhiteSpace(firstName))
            query = query.Where(u => EF.Functions.ILike(u.FirstName, $"%{firstName}%"));

        if (!string.IsNullOrWhiteSpace(lastName))
            query = query.Where(u => EF.Functions.ILike(u.LastName, $"%{lastName}%"));

        if (!string.IsNullOrWhiteSpace(email))
            query = query.Where(u => u.Email == email);

        if (!string.IsNullOrWhiteSpace(username))
            query = query.Where(u => u.Username == username);

        var desc = orderBy.Equals("desc", StringComparison.OrdinalIgnoreCase);

        query = sortBy.ToLower() switch
        {
            "lastname" => desc ? query.OrderByDescending(u => u.LastName) : query.OrderBy(u => u.LastName),
            "firstname" => desc ? query.OrderByDescending(u => u.FirstName) : query.OrderBy(u => u.FirstName),
            "username" => desc ? query.OrderByDescending(u => u.Username) : query.OrderBy(u => u.Username),
            _ => desc ? query.OrderByDescending(u => u.Id) : query.OrderBy(u => u.Id),
        };

        var users = await query
            .Skip(offset)
            .Take(limit)
            .Select(u => new UserResponse
            {
                Id = u.Id,
                FirstName = u.FirstName,
                LastName = u.LastName,
                Email = u.Email,
                Username = u.Username
            })
            .ToListAsync();

        return Ok(users);
    }

    [HttpGet("{id}/todos")]
    public async Task<ActionResult<List<TodoResponse>>> GetUserTodos(
        int id,
        [FromQuery] int limit = 30,
        [FromQuery] int offset = 0,
        [FromQuery] bool? completed = null,
        [FromQuery] string? todo = null,
        [FromQuery] string sortBy = "id",
        [FromQuery] string orderBy = "asc")
    {
        var exists = await _db.Users.AnyAsync(u => u.Id == id);
        if (!exists)
            return NotFound();

        var query = _db.Todos.Where(t => t.UserId == id);

        if (completed.HasValue)
            query = query.Where(t => t.Completed == completed.Value);

        if (!string.IsNullOrWhiteSpace(todo))
            query = query.Where(t => EF.Functions.ILike(t.Description, $"%{todo}%"));

        var desc = orderBy.Equals("desc", StringComparison.OrdinalIgnoreCase);

        query = sortBy.ToLower() switch
        {
            "todo" => desc ? query.OrderByDescending(t => t.Description) : query.OrderBy(t => t.Description),
            _ => desc ? query.OrderByDescending(t => t.Id) : query.OrderBy(t => t.Id),
        };

        var todos = await query
            .Skip(offset)
            .Take(limit)
            .Select(t => new TodoResponse
            {
                Id = t.Id,
                Todo = t.Description,
                Completed = t.Completed
            })
            .ToListAsync();

        return Ok(todos);
    }
}
