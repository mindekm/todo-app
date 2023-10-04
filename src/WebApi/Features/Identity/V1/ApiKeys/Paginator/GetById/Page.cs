namespace WebApi.Features.Identity.V1.ApiKeys.Paginator.GetById;

public sealed class Page
{
    public required int PageSize { get; init; }

    public required Maybe<Dictionary<string, string>> LastEvaluatedKey { get; init; }
}
