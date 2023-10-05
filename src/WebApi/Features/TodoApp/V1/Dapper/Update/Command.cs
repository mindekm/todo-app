namespace WebApi.Features.TodoApp.V1.Dapper.Update;

public sealed class Command
{
    public required Guid Id { get; init; }

    public required string UserId { get; init; }

    public required Maybe<string> Content { get; init; }
}
