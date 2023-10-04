namespace WebApi.Features.TodoApp.V1.DynamoDB.Notes.Paginator.GetById;

public sealed class QueryResult
{
    public required List<Note> Notes { get; init; }

    public required Maybe<string> NextPageToken { get; init; }
}
