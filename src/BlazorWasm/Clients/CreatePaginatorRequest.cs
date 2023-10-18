namespace BlazorWasm.Clients;

public sealed class CreatePaginatorRequest
{
    public int MaxPageSize { get; set; }

    public string SearchCondition { get; set; }
}