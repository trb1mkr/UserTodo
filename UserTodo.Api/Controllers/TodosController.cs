using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using UserTodo.Api.Data;
using UserTodo.Api.Dtos.Api;

namespace UserTodo.Api.Controllers;

[ApiController]
[Route("api/todos")]
public class TodosController : ControllerBase
{
    private readonly ApplicationDbContext _db;

    public TodosController(ApplicationDbContext db)
    {
        _db = db;
    }

    [HttpGet]
    public async Task<ActionResult<List<TodoWithUserResponse>>> GetTodos(
        [FromQuery] int limit = 30,
        [FromQuery] int offset = 0,
        [FromQuery] bool? completed = null,
        [FromQuery] int? userId = null,
        [FromQuery] string? todo = null,
        [FromQuery] string sortBy = "id",
        [FromQuery] string orderBy = "asc")
    {
        var query = _db.Todos.Include(t => t.User).AsQueryable();

        if (completed.HasValue)
            query = query.Where(t => t.Completed == completed.Value);

        if (userId.HasValue)
            query = query.Where(t => t.UserId == userId.Value);

        if (!string.IsNullOrWhiteSpace(todo))
            query = query.Where(t => EF.Functions.ILike(t.Description, $"%{todo}%"));

        var desc = orderBy.Equals("desc", StringComparison.OrdinalIgnoreCase);

        query = sortBy.ToLower() switch
        {
            "userid" => desc ? query.OrderByDescending(t => t.UserId) : query.OrderBy(t => t.UserId),
            "todo" => desc ? query.OrderByDescending(t => t.Description) : query.OrderBy(t => t.Description),
            _ => desc ? query.OrderByDescending(t => t.Id) : query.OrderBy(t => t.Id),
        };

        var todos = await query
            .Skip(offset)
            .Take(limit)
            .Select(t => new TodoWithUserResponse
            {
                Id = t.Id,
                Todo = t.Description,
                Completed = t.Completed,
                User = new UserResponse
                {
                    Id = t.User.Id,
                    FirstName = t.User.FirstName,
                    LastName = t.User.LastName,
                    Email = t.User.Email,
                    Username = t.User.Username
                }
            })
            .ToListAsync();

        return Ok(todos);
    }
}
