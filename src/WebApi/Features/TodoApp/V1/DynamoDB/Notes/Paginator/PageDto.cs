namespace WebApi.Features.TodoApp.V1.DynamoDB.Notes.Paginator;

public sealed class PageDto
{
    public int PageSize { get; set; }

    public Dictionary<string, string> LastEvaluatedKey { get; set; }
}
