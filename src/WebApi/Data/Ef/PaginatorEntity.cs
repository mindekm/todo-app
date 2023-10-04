namespace WebApi.Data.Ef;

public sealed class PaginatorEntity
{
    public string Id { get; set; }

    public int PageSize { get; set; }

    public int? LastResult { get; set; }

    public string SearchCondition { get; set; }

    public DateTimeOffset ExpiresAt { get; set; }
}
