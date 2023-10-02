namespace WebApi.Features.TodoApp.V1.DynamoDB.Notes.Paginator.GetById;

using Utilities;

public sealed class Page
{
    public required int PageSize { get; init; }

    public required Maybe<Dictionary<string, string>> LastEvaluatedKey { get; init; }
}
