namespace WebApi.Data.Redis;

public sealed class NoteEntity
{
    public Guid Id { get; set; }

    public string Title { get; set; }

    public string Content { get; set; }

    public bool IsComplete { get; set; }

    public string CreatedBy { get; set; }

    public DateTimeOffset CreatedAt { get; set; }

    public DateTimeOffset ModifiedAt { get; set; }
}
