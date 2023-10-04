namespace WebApi.Features.TodoApp.V1.EF.Notes.Paginator.GetById;

public sealed class Query
{
    public required string Token { get; init; }

    public required string UserId { get; init; }
}
