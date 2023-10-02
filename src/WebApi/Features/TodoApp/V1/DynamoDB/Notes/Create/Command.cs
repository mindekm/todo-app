namespace WebApi.Features.TodoApp.V1.DynamoDB.Notes.Create;

using Utilities;

public sealed class Command
{
    public required string Title { get; init; }

    public required Maybe<string> Content { get; init; }

    public required string UserId { get; init; }
}
