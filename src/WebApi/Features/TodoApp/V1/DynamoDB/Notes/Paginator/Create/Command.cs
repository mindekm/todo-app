namespace WebApi.Features.TodoApp.V1.DynamoDB.Notes.Paginator.Create;

public sealed class Command
{
    public required int PageSize { get; init; }
}
