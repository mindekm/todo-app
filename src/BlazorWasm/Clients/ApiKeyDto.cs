namespace BlazorWasm.Clients;

public sealed class ApiKeyDto
{
    public Guid Id { get; set; }

    public string Name { get; set; }

    public List<string> Roles { get; set; }

    public DateTimeOffset CreatedAt { get; set; }
}
