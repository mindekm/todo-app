namespace BlazorWasm.Clients;

public sealed class CreateApiKeyPaginatorResponse
{
    public string Token { get; set; }

    public DateTimeOffset ExpiresAt { get; set; }
}
