namespace WebApi.Features.TodoApp.V2.EF.Notes.Delete;

public sealed class Command
{
    public required Guid Id { get; init; }

    public required string UserId { get; init; }
}
