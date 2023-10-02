namespace WebApi.Features.TodoApp.V1.DynamoDB.Notes.BatchCreate;

public sealed class Command
{
    public required string UserId { get; init; }

    public required List<CreateNoteRequest> Requests { get; init; }
}
