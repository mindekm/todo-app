namespace WebApi.Features.TodoApp.V3.EF.Notes.Create;

public sealed class Command
{
    public required string Title { get; init; }

    public required Maybe<string> Content { get; init; }

    public required string UserId { get; init; }
}
