namespace WebApi.Features.TodoApp.V1.EF.Notes.Paginator.Create;

using Utilities;

public sealed class Command
{
    public required int PageSize { get; init; }

    public required Maybe<string> SearchCondition { get; init; }
}
