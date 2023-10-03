namespace WebApi.Features.TodoApp.V1.Redis.Notes.BatchCreate;

public sealed class CommandResult
{
    public required Guid Id { get; init; }

    public required string Title { get; init; }
}
