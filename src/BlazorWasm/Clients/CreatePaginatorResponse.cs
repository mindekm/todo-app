namespace BlazorWasm.Clients;

public sealed class CreatePaginatorResponse
{
    public string Token { get; set; }

    public DateTimeOffset ExpiresAt { get; set; }
}