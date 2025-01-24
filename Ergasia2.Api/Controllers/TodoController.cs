using Ergasia2.Api.Data;
using Ergasia2.Api.Entities;
using Ergasia2.Api.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Ergasia2.Api.Controllers;

[ApiController]
[Route("todos")]
[Authorize]
public class TodoController : ControllerBase
{
    #region DI

    private readonly Ergasia2DbContext _context;
    private readonly UserManager<IdentityUser> _userManager;

    public TodoController(
        Ergasia2DbContext context,
        UserManager<IdentityUser> userManager)
    {
        _context = context;
        _userManager = userManager;
    }

    #endregion

    [HttpGet]
    public async Task<IActionResult> GetTodos()
    {
        var userId = _userManager.GetUserId(User)
            ?? throw new Exception("no user found");

        var todos = await _context.Todos
            .Where(t => t.UserId == userId)
            .Include(t => t.Items)
            .AsNoTracking()
            .ToListAsync();

        return Ok(todos);
    }

    [HttpPost]
    public async Task<IActionResult> CreateTodo([FromBody] TodoBm todoBm)
    {
        var userId = _userManager.GetUserId(User)
            ?? throw new Exception("no user found");

        var todo = new Todo
        {
            Title = todoBm.Title,
            UserId = userId
        };

        _context.Todos.Add(todo);
        await _context.SaveChangesAsync();

        return Ok(new { todo.Id });
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> GetTodo(int id)
    {
        var userId = _userManager.GetUserId(User)
            ?? throw new Exception("no user found"); ;

        var todo = await _context.Todos
            .Include(t => t.Items)
            .FirstOrDefaultAsync(t => t.Id == id && t.UserId == userId);

        if (todo == null)
            return NotFound();

        return Ok(todo);
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> UpdateTodo(int id, [FromBody] TodoBm todoBm)
    {
        var userId = _userManager.GetUserId(User)
            ?? throw new Exception("no user found");

        var todo = await _context.Todos
            .FirstOrDefaultAsync(t => t.Id == id && t.UserId == userId);

        if (todo == null)
            return NotFound();

        todo.Title = todoBm.Title;

        _context.Todos.Update(todo);
        await _context.SaveChangesAsync();

        return Ok();
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteTodo(int id)
    {
        var userId = _userManager.GetUserId(User)
            ?? throw new Exception("No user found");

        var todo = await _context.Todos
            .Include(t => t.Items)
            .FirstOrDefaultAsync(t => t.Id == id && t.UserId == userId);

        if (todo == null)
            return NotFound(new { Message = "Todo not found for the current user." });

        _context.Todos.Remove(todo);
        await _context.SaveChangesAsync();

        return Ok(new { Message = "Todo deleted successfully." });
    }


    [HttpGet("{id}/items/{iid}")]
    public async Task<IActionResult> GetTodoItem(int id, int iid)
    {
        var userId = _userManager.GetUserId(User)
            ?? throw new Exception("no user found");

        var item = await _context.TodoItems
            .FirstOrDefaultAsync(i => i.Id == iid && i.TodoId == id && i.Todo.UserId == userId);

        if (item == null)
            return NotFound();

        return Ok(item);
    }

    [HttpPost("{id}/items")]
    public async Task<IActionResult> AddTodoItem(int id, [FromBody] TodoItemBm itemBm)
    {
        var userId = _userManager.GetUserId(User)
            ?? throw new Exception("no user found");

        var todo = await _context.Todos
            .FirstOrDefaultAsync(t => t.Id == id && t.UserId == userId);

        if (todo == null)
            return NotFound();

        var item = new TodoItem
        {
            Name = itemBm.Name,
            IsComplete = itemBm.IsComplete,
            TodoId = todo.Id
        };

        _context.TodoItems.Add(item);
        await _context.SaveChangesAsync();

        return CreatedAtAction(nameof(GetTodoItem), new { id = todo.Id, iid = item.Id }, item);
    }

    [HttpPut("{id}/items/{iid}")]
    public async Task<IActionResult> UpdateTodoItem(int id, int iid, [FromBody] TodoItemBm itemBm)
    {
        var userId = _userManager.GetUserId(User)
            ?? throw new Exception("no user found");

        var item = await _context.TodoItems
            .FirstOrDefaultAsync(i => i.Id == iid && i.TodoId == id && i.Todo.UserId == userId);

        if (item == null)
            return NotFound();

        item.Name = itemBm.Name;
        item.IsComplete = itemBm.IsComplete;

        _context.TodoItems.Update(item);
        await _context.SaveChangesAsync();

        return Ok();
    }

    [HttpDelete("{id}/items/{iid}")]
    public async Task<IActionResult> DeleteTodoItem(int id, int iid)
    {
        var userId = _userManager.GetUserId(User)
            ?? throw new Exception("no user found");

        var item = await _context.TodoItems
            .FirstOrDefaultAsync(i => i.Id == iid && i.TodoId == id && i.Todo.UserId == userId);

        if (item == null)
            return NotFound();

        _context.TodoItems.Remove(item);
        await _context.SaveChangesAsync();

        return Ok();
    }
}
