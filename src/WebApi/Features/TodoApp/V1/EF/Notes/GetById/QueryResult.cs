namespace WebApi.Features.TodoApp.V1.EF.Notes.GetById;

using Utilities;

public sealed class QueryResult
{
    public required Guid Id { get; init; }

    public required string Title { get; init; }

    public required Maybe<string> Content { get; init; }

    public required DateTimeOffset CreatedAt { get; init; }

    public required DateTimeOffset ModifiedAt { get; init; }

    public required bool IsComplete { get; init; }
}
