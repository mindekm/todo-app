namespace BlazorWasm.Clients;

public sealed class CreateApiKeyRequest
{
    public string Name { get; set; }

    public List<string> Roles { get; set; }
}
