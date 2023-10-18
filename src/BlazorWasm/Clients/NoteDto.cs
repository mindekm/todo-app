namespace BlazorWasm.Clients;

public sealed class NoteDto
{
    public Guid Id { get; set; }

    public string Title { get; set; }

    public string Content { get; set; }

    public DateTimeOffset CreatedAt { get; set; }

    public DateTimeOffset ModifiedAt { get; set; }

    public bool IsComplete { get; set; }
}