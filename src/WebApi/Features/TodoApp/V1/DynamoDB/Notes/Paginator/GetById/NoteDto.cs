namespace WebApi.Features.TodoApp.V1.DynamoDB.Notes.Paginator.GetById;

public sealed class NoteDto
{
    public Guid Id { get; set; }

    public string Title { get; set; }

    public string Content { get; set; }

    public DateTimeOffset CreatedAt { get; set; }

    public DateTimeOffset ModifiedAt { get; set; }

    public bool IsComplete { get; set; }

    public static NoteDto From(Note note)
    {
        return new NoteDto
        {
            Id = note.Id,
            Title = note.Title,
            Content = note.Content.UnwrapOrDefault(),
            CreatedAt = note.CreatedAt,
            ModifiedAt = note.ModifiedAt,
            IsComplete = note.IsComplete,
        };
    }
}
