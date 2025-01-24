using Microsoft.AspNetCore.Identity;

namespace Ergasia2.Api.Entities;

public class Todo
{
    public int Id { get; set; }

    public string Title { get; set; } = null!;

    public string UserId { get; set; } = null!;

    #region Collections
    public List<TodoItem> Items { get; set; } = [];
    #endregion

    #region References
    public IdentityUser User { get; set; }
    #endregion
}
