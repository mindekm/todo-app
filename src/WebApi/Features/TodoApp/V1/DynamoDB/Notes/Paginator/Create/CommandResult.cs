namespace WebApi.Features.TodoApp.V1.DynamoDB.Notes.Paginator.Create;

public sealed class CommandResult
{
    public required string Token { get; init; }

    public required DateTimeOffset ExpiresAt { get; init; }
}
