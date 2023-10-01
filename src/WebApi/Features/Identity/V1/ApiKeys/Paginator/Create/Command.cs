namespace WebApi.Features.Identity.V1.ApiKeys.Paginator.Create;

public sealed class Command
{
    public required int PageSize { get; init; }
}
