namespace WebApi.Features.TodoApp.V1.EF.Notes.Paginator.GetById;

using Utilities;

public sealed class QueryResult
{
    public required List<Note> Notes { get; init; }

    public required Maybe<string> NextPageToken { get; init; }
}
