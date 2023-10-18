namespace BlazorWasm.Clients;

public sealed class GetApiKeyPageResponse
{
    public List<ApiKeyDto> ApiKeys { get; set; }

    public string NextPageToken { get; set; }
}