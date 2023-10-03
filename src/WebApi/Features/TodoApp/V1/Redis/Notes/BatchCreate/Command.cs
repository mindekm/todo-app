namespace WebApi.Features.TodoApp.V1.Redis.Notes.BatchCreate;

public sealed class Command
{
    public required string UserId { get; init; }

    public List<CreateNoteRequest> Requests { get; init; }
}
