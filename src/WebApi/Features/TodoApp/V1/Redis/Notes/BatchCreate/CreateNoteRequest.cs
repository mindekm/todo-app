namespace WebApi.Features.TodoApp.V1.Redis.Notes.BatchCreate;

public sealed class CreateNoteRequest
{
    public required string Title { get; init; }

    public required Maybe<string> Content { get; init; }
}
