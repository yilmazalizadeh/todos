using System.ComponentModel.DataAnnotations;

namespace TravelApp.Entity;

public class Todo
{
    public const int MaxTitleLength = 200;

    public int Id { get; set; }

    [MaxLength(MaxTitleLength)]
    public string? Title { get; set; }
    public DateOnly? DueBy { get; set; }
    public bool IsComplete { get; set; }

    public Todo()
    {
    }

    public Todo(int id, string? title, DateOnly? dueBy = null, bool isComplete = false)
    {
        Id = id;
        Title = title;
        DueBy = dueBy;
        IsComplete = isComplete;
    }
}
