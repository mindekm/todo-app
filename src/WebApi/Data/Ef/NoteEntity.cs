namespace WebApi.Data.Ef;

using NpgsqlTypes;

public sealed class NoteEntity
{
    public int Id { get; set; }

    public Guid ExternalId { get; set; }

    public string Title { get; set; }

    public string Content { get; set; }

    public bool IsComplete { get; set; }

    public string CreatedBy { get; set; }

    public DateTimeOffset CreatedAt { get; set; }

    public DateTimeOffset ModifiedAt { get; set; }

    public NpgsqlTsVector SearchVector { get; set; }
}
