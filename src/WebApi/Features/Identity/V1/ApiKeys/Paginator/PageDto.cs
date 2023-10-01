namespace WebApi.Features.Identity.V1.ApiKeys.Paginator;

public sealed class PageDto
{
    public int PageSize { get; set; }

    public Dictionary<string, string> LastEvaluatedKey { get; set; }
}
