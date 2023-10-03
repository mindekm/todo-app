namespace WebApi.Features.TodoApp.V1.Redis.Notes.Complete;

public sealed class Command
{
    public required Guid Id { get; init; }

    public required string UserId { get; init; }
}
