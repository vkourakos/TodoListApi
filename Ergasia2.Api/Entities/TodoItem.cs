using System.Text.Json.Serialization;

namespace Ergasia2.Api.Entities;

public class TodoItem
{
    public int Id { get; set; }

    public string Name { get; set; } = null!;

    public bool IsComplete { get; set; }
    public int TodoId { get; set; }

    #region References
    [JsonIgnore]
    public Todo Todo { get; set; } = null!;
    #endregion
}

